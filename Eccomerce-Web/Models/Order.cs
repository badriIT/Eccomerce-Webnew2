namespace Eccomerce_Web.Models;

public class Order
{
    public int Id { get; set; }

    public string OrderNumber { get; set; }
    public int UserId { get; set; }

    public User User { get; set; }
    public List<CartItem> Products { get; set; } = new();
}
