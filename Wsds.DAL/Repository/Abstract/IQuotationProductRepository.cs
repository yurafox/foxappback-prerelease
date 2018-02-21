using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IQuotationProductRepository
    {
        IEnumerable<Quotation_Product_DTO> QuotationProducts { get; }
        Quotation_Product_DTO QuotationProduct(long id);
        IEnumerable<Quotation_Product_DTO> GetQuotProdsByProductID(long productID);
    }
}
