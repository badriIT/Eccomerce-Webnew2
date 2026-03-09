namespace Eccomerce_Web.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email);
    }
}
