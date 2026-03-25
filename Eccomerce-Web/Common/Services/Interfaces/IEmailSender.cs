using Eccomerce_Web.Common.Dtos.Responses;

namespace Eccomerce_Web.Common.Services.Interfaces
{
    public interface IEmailSender
    {
        Task<ApiResponse<bool>> SendEmailAsync(string email, string code);
    }
}
