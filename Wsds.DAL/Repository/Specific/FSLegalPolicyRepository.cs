using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Wsds.DAL.Entities.Communication;

namespace Wsds.DAL.Repository.Specific
{
    public class FSLegalPolicyRepository : ILegalPolicyRepository
    {
        private readonly IConfiguration _config;
        public FSLegalPolicyRepository(IConfiguration config)
        {
            _config = config;
        }

        public string GetLegalPolicy(long idLang) {
            string res = null;
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand($"select policy_text as value from legal_policy where id_lang=:lang_id", con)) 
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("lang_id", idLang));
                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        res = dr["value"].ToString();
                    };
                }
                finally
                {
                    con.Close();
                }
            }; 
            return res;
        }
    }
}
