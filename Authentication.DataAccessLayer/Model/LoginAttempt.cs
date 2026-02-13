using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.DataAccessLayer
{
    public class LoginAttempt
    {
        public int LoginAttemptId { get; set; }
        public string IpAddress { get; set; } = string.Empty;

        public string UserAgent { get; set; }= string.Empty;
        public bool IsSuccessful { get; set; }

        public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
        public string? FailureReason { get; set; }

        //Foreign Key (nullable for failed attempts with invalid emails)
         public int? UserId {  get; set; }
        public virtual User? User { get; set; }
    }
}
