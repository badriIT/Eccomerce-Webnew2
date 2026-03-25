using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Eccomerce_Web.Models.Cart;

namespace Eccomerce_Web.Models.User;
using Models.Product;



public class UserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [Required]
    public string Email { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = "User";
    public bool isVerified { get; set; } = false;
    public string ?VerificationCode { get; set; }
    public DateTime? CodeCreatedAt { get; set; }
    public int VerificationAttempts { get; set; }




    public List<Order> Order { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();
    public List<Product> FavoritedProducts { get; set; } = new();

    [JsonIgnore]
    public User User { get; set; }
}