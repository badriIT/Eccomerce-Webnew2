using System.Net;
using System.Net.Mail;
using Eccomerce_Web.Common.Dtos.Responses;
using Eccomerce_Web.Common.Services.Interfaces;
using Eccomerce_Web.Common.Services.ServiceModels;
using Eccomerce_Web.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Eccomerce_Web.Common.Services.implementations
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly DataContext _db;

        public EmailSender(IOptions<EmailSettings> settings, DataContext context)
        {
            _settings = settings.Value;
            _db = context;
        }
        public async Task<ApiResponse<bool>> SendEmailAsync(string email, string code)
        {



            var user = await _db.UserProfiles.FirstOrDefaultAsync(p => p.Email == email);
            if (user == null)
                return ApiResponse<bool>.BadRequest("User not found");


        

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

            user.CodeCreatedAt = DateTime.UtcNow;
            user.VerificationAttempts = 0;

            return  ApiResponse<bool>.Created(true, "email sent");
        }
    }


}
