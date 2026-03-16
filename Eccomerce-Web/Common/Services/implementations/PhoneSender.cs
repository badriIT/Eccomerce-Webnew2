using Eccomerce_Web.Common.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Eccomerce_Web.Common.Services.implementations
{
    public class PhoneSender : IPhoneSender 
    {
        private readonly IConfiguration _configuration;
        public PhoneSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendPhoneMessage(string phoneNumber)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var serviceSid = _configuration["Twilio:VerifyServiceSid"];
            
            TwilioClient.Init(accountSid, authToken);

            var verification = await VerificationResource.CreateAsync(
                to: $"+995{phoneNumber}",
                channel: "sms",
                pathServiceSid: serviceSid
            );
         
        }
        public async Task<bool> VerifyPhoneCode(string phoneNumber, string code)
        {
            var accountSid = _configuration["Twilio:AccountSid"];
            var authToken = _configuration["Twilio:AuthToken"];
            var serviceSid = _configuration["Twilio:VerifyServiceSid"];

            TwilioClient.Init(accountSid, authToken);

            var verificationCheck = await VerificationCheckResource.CreateAsync(
                to: $"+995{phoneNumber}",
                code: code,
                pathServiceSid: serviceSid
            );

            return verificationCheck.Status == "approved";
        }
    }
}
