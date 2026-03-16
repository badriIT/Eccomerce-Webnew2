using System.ComponentModel.DataAnnotations;

namespace Eccomerce_Web.Modules.Auth.Dtos.Request
{
    public class LoginDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}