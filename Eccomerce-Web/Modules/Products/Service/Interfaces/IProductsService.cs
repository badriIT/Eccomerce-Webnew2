using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Enums;
using Eccomerce_Web.Models.Product;
using Eccomerce_Web.Modules.Products.Dtos.Request;

namespace Eccomerce_Web.Modules.Products.Service.Interfaces
{
    public interface IProductsService
    {
        Task<ApiResponse<List<Product>>> GetProducts(Category? category, double? minPrice, double? maxPrice, string? sort, int page, int pageSize);
        Task<ApiResponse<Product>> GetProductById(int id);
        Task<ApiResponse<Product>> AddProduct(ProductDto product);
        Task<ApiResponse<Product>> UpdateProductById(int id, ProductDto updatedProduct);
        Task<ApiResponse<Product>> UpdateProductQuantity(int id, int quantity);
        Task<ApiResponse<bool>> DeleteProductById(int id);
        Task<ApiResponse<List<string>>> GetAllCategories();
    }
}
