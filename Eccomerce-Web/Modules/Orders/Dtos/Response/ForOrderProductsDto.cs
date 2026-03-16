using Eccomerce_Web.Enums;

namespace Eccomerce_Web.Modules.Orders.Dtos.Response
{
    public class ForOrderProductsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Size { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public Category Category { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Description { get; set; }
        public bool IsFavorited { get; set; } = false;
    }
}
