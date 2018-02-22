using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Configuration;

namespace Wsds.DAL.Repository.Specific
{
    public class FSProductRepository : IProductRepository
    {
        private readonly ICacheService<Product_DTO> _csp;
        private readonly IConfiguration _config;
        public FSProductRepository(ICacheService<Product_DTO> csp, IConfiguration config)
        {
            _csp = csp;
            _config = config;
        }

        public IEnumerable<Product_DTO> Products => _csp.Items.Values;

        public Product_DTO Product(long id) => _csp.Item(id);

        public string GetProductDescription(long id) {
            string res = null;
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("select description from products t where t.id = :id", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        res = dr["description"].ToString();
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
