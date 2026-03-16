namespace Eccomerce_Web.Models.Cart;
using Models.Product;
using User;

public class CartItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }


    public User User { get; set; }
    public Product Product { get; set; }


}
