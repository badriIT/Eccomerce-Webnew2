using Eccomerce_Web.Models;

namespace Eccomerce_Web.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }

      
        public List<CartItemsForOrderDto> Products { get; set; } = new();
    }
}
