using Authentication.BusinessLayer.Interfaces;
using Authentication.DataAccessLayer.Configuration;
using Microsoft.Extensions.Options;
using System.Net;

namespace Authentication.WebApi.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IRedisService _redisService;
        private readonly RateLimitingSettings _settings;
        private readonly ILogger<RateLimitMiddleware> _logger;

        public RateLimitMiddleware(
            RequestDelegate next,
            IRedisService redisService,
            IOptions<RateLimitingSettings> settings,
            ILogger<RateLimitMiddleware> logger)
        {
            _next = next;
            _redisService = redisService;
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = GetClientIpAddress(context);

            // Check if IP is blocked
            var blockedKey = $"blocked_ip:{ipAddress}";
            if (await _redisService.ExistsAsync(blockedKey))
            {
                _logger.LogWarning("Blocked IP attempted access: {IP}", ipAddress);
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                await context.Response.WriteAsJsonAsync(new { message = " Your IP is temporarily blocked due to repeated rate limit violations." });
                return;
            }

            // Rate limiting for API route
            var apiRateKey = $"api_rate_limit:{ipAddress}";
            var apiCalls = await _redisService.IncrementAsync(apiRateKey); // if there is no key exist  , it will auto create  key and increments value of key and return 1 as default value was 0.
                                                                           //if key exist then just increment the curent value of key
            if(apiCalls == 1) 
            {
                await _redisService.KeyExpireAsync(apiRateKey, TimeSpan.FromMinutes(1));
            }

            if (apiCalls > _settings.ApiCallsPerMin)
            {
                _logger.LogWarning("Rate limit exceeded by IP: {IP}", ipAddress);

                // Block IP for repeated violations
                var violationsKey = $"rate_limit_violations:{ipAddress}";
                var violations = await _redisService.IncrementAsync(violationsKey);

                if (violations >= _settings.MaxFailedAttempts)
                {
                    await _redisService.SetStringAsync(blockedKey, "1", TimeSpan.FromMinutes(_settings.LockoutDurationMin));
                    _logger.LogWarning("IP blocked due to repeated violations: {IP}", ipAddress);
                }

                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.Headers["Retry-After"] = TimeSpan.FromMinutes(1).TotalSeconds.ToString();
                await context.Response.WriteAsJsonAsync(new { message = "Rate limit exceeded. Please try again later." });
                return;
            }

            // Added Custom rate limit headers
            context.Response.Headers["X-RateLimit-Limit"] = _settings.ApiCallsPerMin.ToString();//  these are the custom made headers
            context.Response.Headers["X-RateLimit-Remaining"] = (_settings.ApiCallsPerMin - apiCalls).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = TimeSpan.FromMinutes(1).TotalSeconds.ToString();

            await _next(context);
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress.Split(',')[0];
            }

            ipAddress = context.Request.Headers["X-Real-IP"].ToString();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                return ipAddress;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

    }
}
