using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IQuotationRepository
    {
        IEnumerable<Quotation_DTO> Quotations { get; }
        Quotation_DTO Quotation(long id);
    }
}
