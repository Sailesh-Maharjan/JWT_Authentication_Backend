using Authentication.BusinessLayer.Interfaces;
using Authentication.DataAccessLayer;
using Authentication.DataAccessLayer.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Authentication.BusinessLayer.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IOptions<JwtSettings> jwtsettings, ILogger<TokenService> logger)
        {
            _jwtSettings = jwtsettings.Value;
            _logger = logger;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim("user_id", user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("first_name", user.FirstName),
                new Claim("middle_name", user.MiddleName??""),
                new Claim("last_name", user.LastName??""),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("TokenId", Guid.NewGuid().ToString()),
                new Claim("email_verified", user.IsEmailVerified.ToString().ToLower())

            };

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secretkey));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMin)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        public DateTime GetTokenExpiryTime(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.ValidTo; // by default ValidTo returns the expiry property value from JwtSecurityToken object (simply from token)
        }

        public ClaimsPrincipal ValidateToken(string token, bool validateLifetime = true)
        {
            try
            {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secretkey);

                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = validateLifetime,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return principal;
            }


            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                throw;
            }
           
        }
    }
}
