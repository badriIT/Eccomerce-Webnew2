using System.ComponentModel.DataAnnotations;

namespace Eccomerce_Web.Modules.User.Dtos.Response
{
    public class OnlyUserInfoDto
    {

        public int Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public bool isVerified { get; set; }

        public string Role { get; set; } = "User";

    }
}
