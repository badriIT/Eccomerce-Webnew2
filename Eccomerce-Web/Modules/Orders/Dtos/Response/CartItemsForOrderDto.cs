using Eccomerce_Web.Models;

namespace Eccomerce_Web.Modules.Orders.Dtos.Response
{
    public class CartItemsForOrderDto
    {

        public int ChoosedProductId { get; set; }
        public int SelectedQuantity { get; set; }

        public ForOrderProductsDto Product { get; set; }
    }
}
