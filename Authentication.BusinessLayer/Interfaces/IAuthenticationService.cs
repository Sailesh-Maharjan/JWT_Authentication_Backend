using Authentication.BusinessLayer.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.BusinessLayer.Interfaces
{
    public  interface IAuthenticationService
    {
        Task<AuthenticationResponseVM?> RegisterAsync(RegisterRequestVM request, string ipAdress, string userAgent);
        Task<AuthenticationResponseVM> LoginAsync(LoginRequestVM request, string ipAddress, string userAgent);
        Task<AuthenticationResponseVM?> RefreshTokenAsync(string refreshToken, string ipAddress, string userAgent);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<bool> IsUserLockedOutAsync(string email);
        Task<bool> HasExceededLoginAttemptsAsync(string ipAddress);
    }
}
