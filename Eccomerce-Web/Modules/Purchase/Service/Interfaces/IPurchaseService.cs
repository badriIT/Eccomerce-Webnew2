using Eccomerce_Web.Common.Dtos.Responses;

namespace Eccomerce_Web.Modules.Purchase.Service.Interfaces
{
    public interface IPurchaseService
    {
        Task<ApiResponse<bool>> Buy(int userId, string orderNumber);
    }
}
