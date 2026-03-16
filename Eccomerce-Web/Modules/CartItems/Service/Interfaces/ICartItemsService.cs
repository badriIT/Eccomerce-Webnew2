using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Modules.CartItems.Dtos.Response;

namespace Eccomerce_Web.Modules.CartItems.Service.Interfaces
{
    public interface ICartItemsService
    {
        Task<ApiResponse<List<ForCartItems>>> GetCartItems(int userId);
        Task<ApiResponse<bool>> AddToCart(int userId, int productId, int quantity);
        Task<ApiResponse<bool>> UpdateCartItem(int userId, int cartItemId, int quantity);
        Task<ApiResponse<bool>> ClearCart(int userId);
        Task<ApiResponse<bool>> RemoveFromCart(int userId, int cartItemId);
    }
}
