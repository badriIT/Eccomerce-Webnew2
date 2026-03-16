namespace Eccomerce_Web.Common.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string code);
    }
}
