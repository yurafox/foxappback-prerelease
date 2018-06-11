using System;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Wsds.DAL.Services.Abstract;

namespace Wsds.DAL.Services.Specific
{
    public class FSAuthSender:IAuthSender
    {
        private readonly IConfiguration _configuration;
        private string OraConnectionString { get;}
        private int PasswordTemplate => 2728;
            
        public FSAuthSender(IConfiguration configuration)
        {
            _configuration = configuration;
            OraConnectionString = _configuration.GetConnectionString("MainDataConnection");
        }
        public void SendSms(string phone, string message)
        {
            using (IDbConnection db = new OracleConnection(OraConnectionString))
            {
                var oraQuery = @"begin aspnetuser.pkg_sms.send_sms@asm(p_tpl_id => :tpl, p_phone => :phone," +
                               " p_values => :message, p_commit => 1); end;";

                db.Execute(oraQuery, new { tpl=PasswordTemplate, phone, message});

            }
        }

        public string GenerateRandomPassword(int length)
        {
            const string valid = "1234567890";
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
