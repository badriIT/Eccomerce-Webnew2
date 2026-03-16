using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Eccomerce_Web.Models;

namespace Eccomerce_Web.Dtos
{
    public class WholeUserProfileDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        [Required]
        public string Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        public string Role { get; set; } = "User";

        public List<ForWholeProfileOrderDto> Order { get; set; } = new();

      
        public List<ForCartItems> CartItems { get; set; } = new();

        [NotMapped]
        public double CartTotal => CartItems?.Sum(ci => ci.Product != null ? ci.Product.Price * ci.SelectedQuantity : 0) ?? 0;

        public List<ForProfileProductDto> FavoritedProducts { get; set; } = new();

    }
}
