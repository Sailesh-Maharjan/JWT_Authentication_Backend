namespace Authentication.WebApi.Middleware
{
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityMiddleware> _logger;

        public SecurityMiddleware(RequestDelegate next, ILogger<SecurityMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Add security headers
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";

            // Remove server header
            context.Response.Headers.Remove("Server");

            // Log suspicious request
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            if (IsSuspiciousRequest(context))
            {
                _logger.LogWarning("Suspicious request detected from IP: {IP}, User-Agent: {UA}", ipAddress, userAgent);
            }

            await _next(context);
        }

        private static bool IsSuspiciousRequest(HttpContext context)
        {
            var suspiciousPatterns = new string[]
            {
                "sqlmap", "nmap", "nikto",
                "<script>", "javascript:",
                "../", "..\\", "/etc/"
            };

            var requestContent = $"{context.Request.Path}{context.Request.QueryString}{context.Request.Headers}";

            foreach (var pattern in suspiciousPatterns)
            {
                if (requestContent.Contains(pattern,StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

    }
}
