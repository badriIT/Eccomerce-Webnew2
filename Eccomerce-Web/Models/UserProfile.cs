using System.Text.Json.Serialization;

namespace Eccomerce_Web.Models;

public class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public string Email { get; set; }
    public string? FullName { get; set; } 
    public string? PhoneNumber { get; set; }



    public List<Order> Order { get; set; } = new();
    public List<CartItem> CartItems { get; set; } = new();
    public List<Product> FavoritedProducts  { get; set; } = new();



    [JsonIgnore]
    public User User { get; set; }
}
