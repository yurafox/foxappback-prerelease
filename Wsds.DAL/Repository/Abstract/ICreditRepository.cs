using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ICreditRepository
    {
        IEnumerable<CreditProduct_DTO> CreditProducts { get; }
        CreditProduct_DTO CreditProduct(long id);
        ProductSupplCreditGrade_DTO GetProductCreditSize(long idProduct, long idSupplier);
    }
}
