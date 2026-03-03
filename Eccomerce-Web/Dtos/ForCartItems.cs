namespace Eccomerce_Web.Dtos
{
    public class ForCartItems
    {
           public int CartItemIdInCart { get; set; }
           public int SelectedQuantity { get; set; }

           public ForProfileProductDto Product { get; set; }

    }
}
