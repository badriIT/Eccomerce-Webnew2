using System.ComponentModel.DataAnnotations;

namespace Eccomerce_Web.Dtos
{
    public class UserProfileUPTDto
    {
        [Required]
        [EmailAddress]
        [StringLength(100, MinimumLength = 5)]
        public  string Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
