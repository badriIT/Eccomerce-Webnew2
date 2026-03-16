using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Modules.Orders.Dtos.Response;

namespace Eccomerce_Web.Modules.Orders.Service.Interfaces
{
    public interface IOrdersService
    {
        Task<ApiResponse<List<OrderDto>>> GetAllOrders(int userId);
        Task<ApiResponse<bool>> CreateSingleItemOrder(int userId, int productId, int quantity);
        Task<ApiResponse<OrderDto>> CreateOrderFromCart(int userId);
        Task<ApiResponse<bool>> UpdateOrder(int userId, int orderId, int ordersProductsId, int quantity);
        Task<ApiResponse<bool>> RemoveOrder(int userId, int orderId);
    }
}
