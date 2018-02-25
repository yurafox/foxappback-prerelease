using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IFinRepository
    {
        IEnumerable<Enum_Pmt_Method_DTO> PaymentMethods { get; }
        Enum_Pmt_Method_DTO PaymentMethod(long id);
    }
}
