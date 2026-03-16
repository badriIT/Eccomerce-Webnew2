namespace Eccomerce_Web.Modules.CartItems.Dtos.Response
{
    public class ForCartItems
    {
           public int CartItemIdInCart { get; set; }
           public int SelectedQuantity { get; set; }

           public ForProfileProductDto Product { get; set; }

    }
}
