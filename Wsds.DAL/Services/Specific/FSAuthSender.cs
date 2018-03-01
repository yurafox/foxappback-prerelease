using System;
using System.Text;
using Wsds.DAL.Services.Abstract;

namespace Wsds.DAL.Services.Specific
{
    public class FSAuthSender:IAuthSender
    {
        public void SendSms(string phone, string message)
        {
            throw new NotImplementedException();
        }

        public string GenerateRandomPassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
    }
}
