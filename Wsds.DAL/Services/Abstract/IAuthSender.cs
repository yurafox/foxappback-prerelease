namespace Wsds.DAL.Services.Abstract
{
    public interface IAuthSender
    {
        void SendSms(string phone, string message);
        string GenerateRandomPassword(int length);
    }
}
