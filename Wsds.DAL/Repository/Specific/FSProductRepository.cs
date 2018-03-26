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

        
        //This is the fastest search method
        //TODO: In production system don't forget to change the name of search field to indexed text field, which includes
        //all nessesary text, describing current item (product name, description, key features etc.)
        public IEnumerable<Product_DTO> SearchProducts(string srchString)
        {
            string fldName = "name";  
            IDictionary<string, OracleParameter> _dict = new Dictionary<string, OracleParameter>();
            string[] tokens = srchString.Trim().Split(' ').Distinct().ToArray();
            int i = 0;
            foreach (string s in tokens) {
                if (s.Trim() != "")
                { 
                    var _pname = "param" + i.ToString();
                    var str = "regexp_like (" + fldName + ", :" +_pname + ", 'i')";
                    _dict.Add(str, new OracleParameter(_pname, s.Trim()));
                }
                i++;
            }

            /* 
             * Search by entity filter expression - this aproach is very slow because of clob reading :(
            
            var prodCnfg = EntityConfigDictionary.GetConfig("products");
            var prov = new EntityProvider<Product_DTO>(prodCnfg);
            return prov.GetItems(
                                    string.Join(" and ", _dict.Keys),
                                    _dict.Values.ToArray()
                                 ); 
           */

            //And this search rocks :)
            var stmt = "select t.id from products t " +
                       "where " + string.Join(" and ", _dict.Keys) +
                       " and t.price>0 order by nvl(t.popularity,0) desc, t.price desc";
            var ConnString = _config.GetConnectionString("MainDataConnection");

            List <Product_DTO> res = new List<Product_DTO>();

            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    cmd.Parameters.AddRange(_dict.Values.ToArray());
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    dr.FetchSize = cmd.RowSize * 100;
                    var j = 0;
                    while (dr.Read() && (j<=99))
                    {
                        var item = _csp.Item(Int64.Parse(dr["id"].ToString()));
                        if (item != null)
                        { 
                            res.Add(item);
                            j++;
                        }
                    };
                }
                finally
                {
                    con.Close();
                }
            };

            return res;
        }

        private bool IsFound(string stringToSearch, string stringToFind) {
            string[] tokens = stringToFind.Split(' ').Distinct().ToArray();
            foreach (string s in tokens)
            {
                var found = stringToSearch.ToLower().Contains(s.ToLower());
                if (!found)
                    return false;
            }
            return true;
        }
        
        //searching in in-memory cache of ~50k products takes up to 60 seconds :(
        public IEnumerable<Product_DTO> SearchProductsInCache(string srchString)
        {
            return _csp.Items.Values.Where(x => IsFound(x.name, srchString));
        }
    }
}
