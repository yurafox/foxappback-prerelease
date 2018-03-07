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

        public IEnumerable<string> GetProductImages(long id)
        {
            List<string> res = new List<string>();
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("select app_core.Get_Image_Root_Url||Location as value " +
                "from PRODUCT_FILES t where t.id_product = :id order by List_index", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        res.Add(dr["value"].ToString());
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

        public IEnumerable<Product_DTO> SearchProducts(string srchString)
        {
            string fldName = "name";
            IDictionary<string, OracleParameter> _dict = new Dictionary<string, OracleParameter>();
            string[] tokens = srchString.Split(' ').Distinct().ToArray();
            int i = 0;
            foreach (string s in tokens) {
                var _pname = "param" + i.ToString();
                var str = "regexp_like (" + fldName + ", :" +_pname + ", 'i')";
                _dict.Add(str, new OracleParameter(_pname, s));
                i++;
            }

            var prodCnfg = EntityConfigDictionary.GetConfig("products");
            var prov = new EntityProvider<Product_DTO>(prodCnfg);
            return prov.GetItems(
                                    string.Join(" and ", _dict.Keys),
                                    _dict.Values.ToArray()
                                 );
        }
    }
}
