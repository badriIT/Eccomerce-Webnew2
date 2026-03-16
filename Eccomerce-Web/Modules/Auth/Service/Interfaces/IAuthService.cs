using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.CORE;
using Eccomerce_Web.Modules.Auth.Dtos.Request;

namespace Eccomerce_Web.Modules.Auth.Service.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<bool>> VerifyEmail(string code, string email);
        Task<ApiResponse<bool>> SendEmailVerificationCode(string email);
        Task<ApiResponse<string>> Register(RegisterDto user);
        Task<ApiResponse<object>> Login(LoginDto login);
    }
}
