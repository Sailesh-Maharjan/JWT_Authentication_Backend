using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.BusinessLayer.DTO
{
    public class UserInfoVM
    {
        public int Id { get; set; }
        public string Email { get; set; }= string.Empty;
        public string FirstName {get; set; } = string.Empty;
        public string? MiddleName {get; set;} 
        public string? LastName {get; set;} 
        public DateTime CreatedAt {get; set;} 
        public DateTime? LastLoginAt {get; set;} 

    }
}
