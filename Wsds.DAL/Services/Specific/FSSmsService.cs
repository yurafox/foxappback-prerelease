using System.Threading.Tasks;
using Wsds.DAL.Services.Abstract;

namespace Wsds.DAL.Services.Specific
{
    public class FSSmsService:ISmsService
    {
        private IAuthSender _sender;

        public FSSmsService(IAuthSender sender)
        {
            _sender = sender;
        } 
        public Task SendAuthSmsAsync(string number, string message)
        {
            _sender.SendSms(number,message);
            return Task.FromResult(0);
        }

        public string GetAuthTempPswd(int length)
        {
           return _sender.GenerateRandomPassword(length);
        } 
    }
}
