using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSCreditRepository : ICreditRepository
    {
        private readonly ICacheService<CreditProduct_DTO> _csCredProd;
        private readonly IConfiguration _config;

        public FSCreditRepository(ICacheService<CreditProduct_DTO> csCredProd, IConfiguration config) {
            _csCredProd = csCredProd;
            _config = config;
        } 
        public IEnumerable<CreditProduct_DTO> CreditProducts => _csCredProd.Items.Values;

        public CreditProduct_DTO CreditProduct(long id) => _csCredProd.Item(id);

        public IEnumerable<ProductSupplCreditGrade_DTO> GetProductCreditSize(long idProduct, long idSupplier)
        {
            List<ProductSupplCreditGrade_DTO> res = new List<ProductSupplCreditGrade_DTO>();
            
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("select t.id, t.parts_pmt_cnt, t.credit_size from PRODUCT_SUPPL_CREDIT_GRADES t " +
                                               "where t.id_product = :idProduct and t.id_supplier=:idSupplier", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("idProduct", idProduct));
                    cmd.Parameters.Add(new OracleParameter("idSupplier", idSupplier));
                    OracleDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ProductSupplCreditGrade_DTO item = new ProductSupplCreditGrade_DTO();
                        item.id = Convert.ToInt64(dr["id"].ToString());
                        item.idProduct = idProduct;
                        item.idSupplier = idSupplier;
                        item.partsPmtCnt = Convert.ToInt32(dr["parts_pmt_cnt"].ToString());
                        item.creditSize = Convert.ToInt32(dr["credit_size"].ToString());
                        res.Add(item);
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
