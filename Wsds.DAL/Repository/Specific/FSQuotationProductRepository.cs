using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSQuotationProductRepository : IQuotationProductRepository
    {
        private ICacheService<Quotation_Product_DTO> _csqp;
        public FSQuotationProductRepository(ICacheService<Quotation_Product_DTO> csqp) =>
                _csqp = csqp;

        public IEnumerable<Quotation_Product_DTO> QuotationProducts => _csqp.Items.Values;

        public Quotation_Product_DTO QuotationProduct(long id) => _csqp.Item(id);

        public IEnumerable<Quotation_Product_DTO> GetQuotProdsByProductID(long productID) {

            var qpCnfg = EntityConfigDictionary.GetConfig("quotation_product");
            var prov = new EntityProvider<Quotation_Product_DTO> (qpCnfg);

            return prov.GetItems("id_Product = :prod",  new OracleParameter("a", productID));
        }

    }
}
