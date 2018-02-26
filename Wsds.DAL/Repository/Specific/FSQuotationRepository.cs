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
    public class FSQuotationRepository : IQuotationRepository
    {
        private ICacheService<Quotation_DTO> _csQuot;

        public FSQuotationRepository(ICacheService<Quotation_DTO> csQuot) => _csQuot = csQuot;

        public IEnumerable<Quotation_DTO> Quotations => _csQuot.Items.Values;

        public Quotation_DTO Quotation(long id) => _csQuot.Item(id);
    }
}
