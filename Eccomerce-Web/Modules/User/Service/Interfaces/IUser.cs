using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Models.User;
using Eccomerce_Web.Modules.User.Dtos.Response;

namespace Eccomerce_Web.Modules.User.Service.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<OnlyUserInfoDto>> GetCurrentUserProfile(int userId);
        Task<ApiResponse<bool>> AddPhone(int userId, string phone);
        Task<ApiResponse<bool>> VerifyPhone(int userId, string phone, string code);
        Task<ApiResponse<UserProfile>> DeleteCurrentUser(int userId);
    }
}