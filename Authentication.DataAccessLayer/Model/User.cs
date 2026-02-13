using System.ComponentModel.DataAnnotations.Schema;

namespace Authentication.DataAccessLayer
{
    [Table("users")]
    public class User
    {
        public int UserId {  get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set;}
        public bool IsEmailVerified { get; set; } 

        public bool IsActive { get; set; }
        public int FailedLoginAttempts { get; set; } 

        public DateTime? LockoutEnd { get;set; }

        //navigation property
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public virtual ICollection<LoginAttempt> LoginAttempts { get; set; } = new List<LoginAttempt>();
     

    }
}
