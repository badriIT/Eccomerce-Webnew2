namespace Eccomerce_Web.Services.Interfaces
{
    public interface IPhoneSender
    {
        Task SendPhoneMessage(string phoneNumber);
    }
}
