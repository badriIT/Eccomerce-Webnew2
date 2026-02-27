using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Eccomerce_Web.Dtos
{
    public class RegisterDto
    {

        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 5)]
        public string Email { get; set; }
        //[RegularExpression(@".*@gmail\.com$", ErrorMessage = "Email must be Gmail")]



        [Required]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,100}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character."
        )]
        public string Password { get; set; }
    }
}