namespace Eccomerce_Web.Common.Services.Interfaces
{
    public interface IPhoneSender
    {
        Task SendPhoneMessage(string phoneNumber);

        Task<bool> VerifyPhoneCode(string phoneNumber, string code);
    }
}
