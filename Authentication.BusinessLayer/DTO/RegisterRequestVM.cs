using System.ComponentModel.DataAnnotations;

namespace Authentication.BusinessLayer.DTO
{
    public class RegisterRequestVM
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
      /*  [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$")]*/
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;


     /*   [Required]*/
        [MaxLength(100)]
        public string? MiddleName { get; set; } 


       /* [Required]*/
        [MaxLength(100)]
        public string? LastName { get; set; } 

    }


}
