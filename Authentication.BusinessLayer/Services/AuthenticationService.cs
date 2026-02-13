using Authentication.BusinessLayer.DTO;
using Authentication.BusinessLayer.Interfaces;
using Authentication.DataAccessLayer;
using Authentication.DataAccessLayer.AppDbContext;
using Authentication.DataAccessLayer.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace Authentication.BusinessLayer.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IRedisService _redisService;
        private readonly RateLimitingSettings _rateLimitingSettings;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            ApplicationDbContext context,
            ITokenService tokenService,
            IRedisService redisService,
            IOptions<RateLimitingSettings> rateLimitingSettings,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthenticationService> logger)
        {
            _context = context;
            _tokenService = tokenService;
            _redisService = redisService;
            _rateLimitingSettings = rateLimitingSettings.Value;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthenticationResponseVM?> RegisterAsync(RegisterRequestVM request, string ipAddress, string userAgent)
        {
            await using var _transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: user already exists with email {Email}", request.Email);
                    return null;
                }

                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    MiddleName = request.MiddleName,
                    LastName = request.LastName,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsEmailVerified = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate tokens
                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.UserId,
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDay),
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                _context.RefreshTokens.Add(refreshTokenEntity);
                await _context.SaveChangesAsync();
                await _transaction.CommitAsync();

                _logger.LogInformation("User registered successfully: {Email}", user.Email);

                return new AuthenticationResponseVM
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMin),
                    RefreshTokenExpiry = refreshTokenEntity.ExpiryDate,
                    User = new UserInfoVM
                    {
                        Id = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = null
                    }
                };
            }
            catch (Exception ex)
            {
                await _transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred during user registration");
                throw;
            }
        }

        public async Task<AuthenticationResponseVM> LoginAsync(LoginRequestVM request, string ipAddress, string userAgent)
        {
            try
            {
                int remainingLoginAttempts = 0;
                if (await HasExceededLoginAttemptsAsync(ipAddress))
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", ipAddress);
                    return new AuthenticationResponseVM
                    {
                        Success = false,
                        ErrorMessage = $"Rate limit exceeded.",
                        IsRateLimited = true
                    };
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

                var loginAttempt = new LoginAttempt
                {
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    UserId = user?.UserId,
                    AttemptedAt = DateTime.UtcNow
                };

                if (user == null)
                {
                    loginAttempt.IsSuccessful = false;
                    _context.LoginAttempts.Add(loginAttempt);
                    await _context.SaveChangesAsync();
                    await IncrementLoginAttemptAsync(ipAddress);
                    return new AuthenticationResponseVM
                    {
                        Success = false,
                        ErrorMessage = "Invalid credentials"
                    };
                }

                if (await IsUserLockedOutAsync(user.Email))
                {
                    loginAttempt.IsSuccessful = false;
                    _context.LoginAttempts.Add(loginAttempt);
                    await _context.SaveChangesAsync();
                    return new AuthenticationResponseVM
                    {
                        Success = false,
                        ErrorMessage = "Too many login attempts. Try again later.",
                        IsLockedOut = true
                    };
                }

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    loginAttempt.IsSuccessful = false;
                    _context.LoginAttempts.Add(loginAttempt);

                    user.FailedLoginAttempts += 1;
                     remainingLoginAttempts = _rateLimitingSettings.MaxFailedAttempts - user.FailedLoginAttempts;

                    if (user.FailedLoginAttempts >= _rateLimitingSettings.MaxFailedAttempts)
                    {
                        user.LockoutEnd = DateTime.UtcNow.AddMinutes(_rateLimitingSettings.LockoutDurationMin);
                        _logger.LogWarning("User {Email} locked out due to multiple failed logins", user.Email);
                    }

                    await _context.SaveChangesAsync();
                    await IncrementLoginAttemptAsync(ipAddress);
                    return new AuthenticationResponseVM
                    {
                        Success = false,
                        ErrorMessage = "Invalid credentials",
                        RemainingLoginAttempts = remainingLoginAttempts
                    };
                }

                // Successful login
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                user.LastLoginAt = DateTime.UtcNow;

                loginAttempt.IsSuccessful = true;
                _context.LoginAttempts.Add(loginAttempt);

                var accessToken = _tokenService.GenerateAccessToken(user);
                var refreshToken = _tokenService.GenerateRefreshToken();

                var oldTokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == user.UserId && !rt.IsRevoked)
                    .ToListAsync();

                foreach (var oldToken in oldTokens)
                {
                    oldToken.IsRevoked = true;
                    oldToken.RevokedAt = DateTime.UtcNow;
                }

                var newRefreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.UserId,
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDay),
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                _context.RefreshTokens.Add(newRefreshTokenEntity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {Email} logged in successfully", user.Email);

                return new AuthenticationResponseVM
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMin),
                    RefreshTokenExpiry = newRefreshTokenEntity.ExpiryDate,
                    Success =true,
                    ErrorMessage = "Login Successfully",
                    User = new UserInfoVM
                    {
                        Id = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", request.Email);
                throw;
            }
        }

        public async Task<AuthenticationResponseVM?> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent)
        {
            try
            {
                var tokenEntity = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

                if (tokenEntity == null)
                {
                    _logger.LogWarning("Invalid refresh token");
                    return null;
                }

                var user = tokenEntity.User;
                var newAccessToken = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenEntity.IsRevoked = true;
                tokenEntity.RevokedAt = DateTime.UtcNow;
                tokenEntity.ReplacedByToken = newRefreshToken;

                var newRefreshTokenEntity = new RefreshToken
                {
                    Token = newRefreshToken,
                    UserId = user.UserId,
                    ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDay),
                    IpAddress = ipAddress,
                    UserAgent = userAgent
                };

                _context.RefreshTokens.Add(newRefreshTokenEntity);
                await _context.SaveChangesAsync();

                return new AuthenticationResponseVM
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                    AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMin),
                    RefreshTokenExpiry = newRefreshTokenEntity.ExpiryDate,
                    User = new UserInfoVM
                    {
                        Id = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
                throw;
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var tokenEntity = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (tokenEntity == null)
                    return false;

                tokenEntity.IsRevoked = true;
                tokenEntity.RevokedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Token revoked successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking token");
                return false;
            }
        }

        public async Task<bool> IsUserLockedOutAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user?.LockoutEnd.HasValue == true && user.LockoutEnd > DateTime.UtcNow;
        }

        public async Task<bool> HasExceededLoginAttemptsAsync(string ipAddress)
        {
            var attemptsKey = $"login:attempts:{ipAddress}";
            var attempts = await _redisService.GetStringAsync(attemptsKey);

            if (int.TryParse(attempts, out int attemptCount))
            {
                return attemptCount >= _rateLimitingSettings.LoginAttemptsPerMin;
            }

            return false;
        }

        private async Task IncrementLoginAttemptAsync(string ipAddress)
        {
            var attemptsKey = $"login:attempts:{ipAddress}";
            await _redisService.IncrementAsync(attemptsKey, TimeSpan.FromMinutes(1));
        }
    }
}
