using Eccomerce_Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Verify.V2.Service;

namespace Eccomerce_Web.Services.Implementations
{
    public class PhoneSender : IPhoneSender /// anu bevri ar gamoiyeno sul 1k jer segvidzlia gamoviyenot gadadi jsonshi
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
    }
}
