using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ICurrencyRepository
    {
        IEnumerable<Currency_DTO> Currencies { get; }
        IEnumerable<CurrencyRate_DTO> CurrencyRate { get; }
        Currency_DTO Currency(long id);
    }
}
