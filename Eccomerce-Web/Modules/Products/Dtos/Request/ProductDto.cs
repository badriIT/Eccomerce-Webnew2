using System.ComponentModel.DataAnnotations;
using Eccomerce_Web.Enums;

namespace Eccomerce_Web.Modules.Products.Dtos.Request
{
    public class ProductDto
    {

        [Required]
        public string Name { get; set; }

        [Required]
        public string Size { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public double Price { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public double Quantity { get; set; }
        public Category Category { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Description { get; set; }
       
    }
}
