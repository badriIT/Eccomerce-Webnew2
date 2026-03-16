using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Dtos;
using Eccomerce_Web.Modules.User.Dtos.Response;

namespace Eccomerce_Web.Modules.Admin.Services.Interfaces
{
    public interface IAdminService
    {
        Task<ApiResponse<OrdersAdminDto>> GetAllSuccessfulOrders();
        Task<ApiResponse<List<OnlyUserInfoDto>>> GetAllUsers();
    }
}
