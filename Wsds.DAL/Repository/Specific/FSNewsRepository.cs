using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Linq;

namespace Wsds.DAL.Repository.Specific
{
    public class FSNewsRepository : INewsRepository
    {
        private ICacheService<News_DTO> _csb;
        private readonly IConfiguration _config;

        public FSNewsRepository(ICacheService<News_DTO> csb, IConfiguration config)
        {
            _csb = csb;
            _config = config;
        }

        public IEnumerable<News_DTO> News => _csb.Items.Values;

        public string GetNewsDescription(long id)
        {
            string res = null;
            var ConnString = _config.GetConnectionString("MainDataConnection");
            var replacePattern = "replace(article, '<img src=\"img.aspx?id=', '<img src=\"http://www.foxtrot.com.ua/img.aspx?id=')";
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand($"select {replacePattern} as article from news t where t.id = :id", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        res = dr["article"].ToString();
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

        public IEnumerable<News_DTO> SearchNewsInCache(int categoryId)
        {
            return _csb.Items.Values.Where(x => x.categoryId == categoryId);
        }

    }
}
