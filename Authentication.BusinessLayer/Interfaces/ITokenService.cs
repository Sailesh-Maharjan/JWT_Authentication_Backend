using Authentication.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.BusinessLayer.Interfaces
{
    public  interface ITokenService
    {

        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal ValidateToken(string token, bool validateLifetime = true); // not used , instead directly configured in program.cs
        DateTime GetTokenExpiryTime(string token);
    }
}
