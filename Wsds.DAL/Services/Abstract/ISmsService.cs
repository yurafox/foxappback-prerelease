using System.Threading.Tasks;

namespace Wsds.DAL.Services.Abstract
{
    public interface ISmsService
    {
        Task SendAuthSmsAsync(string number, string message);
        string GetAuthTempPswd(int length);
    }
}
