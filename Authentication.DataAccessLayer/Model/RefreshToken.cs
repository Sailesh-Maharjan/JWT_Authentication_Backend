using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataAccessLayer
{
    public  class RefreshToken
    {
        public int RefreshTokenId {  get; set; }    
        public string Token {  get; set; }=string.Empty;

        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt {  get; set; }

        public string? ReplacedByToken {  get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent {  get; set; } = string.Empty;

        //Foreign Key
        public int UserId {  get; set; }
        public virtual User User { get; set; } = null!;

        //Read only property
        public bool IsExpired => DateTime.UtcNow >= ExpiryDate;

        public bool IsActive
        {
            get
            {
                if (!IsRevoked && !IsExpired)
                {
                    return true;
                }
                return false;
            }
        }


    }
}
