using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ICurrencyRepository
    {
        IEnumerable<Currency_DTO> Currencies { get; }
        Currency_DTO Currency(long id);
    }
}
