using Eccomerce_Web.Common.Services.Interfaces;
using Eccomerce_Web.Common.Services.ServiceModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Eccomerce_Web.Common.Services.implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;

        public EmailSender(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task SendEmailAsync(string email, string code)
        {
            var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _settings.Email,
                    _settings.Password)
            };

            _settings.Code = code;
            string htmlBody = File.ReadAllText(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HTML", "verification-email.html")
            ).Replace("{{CODE}}", _settings.Code);

            var mail = new MailMessage(
                _settings.Email,
                email,
                _settings.Code,
                htmlBody
            );
            mail.IsBodyHtml = true;

            await client.SendMailAsync(mail);
        }
    }


}
