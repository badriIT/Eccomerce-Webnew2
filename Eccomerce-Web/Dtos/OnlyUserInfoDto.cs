using System.ComponentModel.DataAnnotations;

namespace Eccomerce_Web.Dtos
{
    public class OnlyUserInfoDto
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "User";

    }
}
