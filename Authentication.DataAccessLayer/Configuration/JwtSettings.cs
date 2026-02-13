using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataAccessLayer.Configuration
{
    public class JwtSettings
    {
        public string Secretkey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpirationMin { get; set; } = 15;
        public int RefreshTokenExpirationDay { get; set; } = 7;

    }
}
