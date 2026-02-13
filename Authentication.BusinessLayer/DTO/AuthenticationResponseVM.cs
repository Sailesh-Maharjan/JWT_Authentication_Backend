using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.BusinessLayer.DTO
{
    public class AuthenticationResponseVM
    {
        public string AccessToken {  get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry {  get; set; }
        public DateTime RefreshTokenExpiry {  get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsLockedOut { get; set; }
        public bool IsRateLimited { get; set; }
        public int ? RemainingLoginAttempts { get; set; }
        public UserInfoVM User {  get; set; } = new UserInfoVM();

    }
}
