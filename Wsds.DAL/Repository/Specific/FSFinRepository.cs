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
    public class FSFinRepository : IFinRepository
    {
        private ICacheService<Enum_Pmt_Method_DTO> _csPmtMethod;

        public FSFinRepository(ICacheService<Enum_Pmt_Method_DTO> csPmtMethod) {
            _csPmtMethod = csPmtMethod;
        }

        public IEnumerable<Enum_Pmt_Method_DTO> PaymentMethods => _csPmtMethod.Items.Values.OrderBy(x => x.id);

        public Enum_Pmt_Method_DTO PaymentMethod(long id) => _csPmtMethod.Item(id);
    }
}
