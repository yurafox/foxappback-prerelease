using System.Data;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSPageRepository:IPageRepository
    {
        private readonly ICacheService<Page_DTO> _csPage;
        private readonly IConfiguration _config;


        public FSPageRepository(ICacheService<Page_DTO> csPage, IConfiguration config)
        {
            _csPage = csPage;
            _config = config;
        }

        public Page_DTO GetPageById(long id)
        {
            return _csPage.Item(id);
        }

        public string GetPageOptions(long id)
        {
            string res = "";
            var connString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(connString))
            using (var cmd = new OracleCommand("begin: result:= serialization.pageoptions2json(id_ => :id_); end; ", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 4000;
                    cmd.Parameters.Add(new OracleParameter("id_", id));

                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();
                }
                finally
                {
                    con.Close();
                }
            }

            return res;
        }
    }
}
