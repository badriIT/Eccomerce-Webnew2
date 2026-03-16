using Eccomerce_Web.Modules.CartItems.Dtos.Response;

namespace Eccomerce_Web.Dtos
{
    public class ForWholeProfileOrderDto
    {
        public string OrderNumber { get; set; } 

        public List<ForCartItems> Products { get; set; } = new();
    }
}
