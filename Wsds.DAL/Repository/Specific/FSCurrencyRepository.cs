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
    public class FSCurrencyRepository : ICurrencyRepository
    {
        private ICacheService<Currency_DTO> _csc;

        public FSCurrencyRepository(ICacheService<Currency_DTO> csc) => _csc = csc;

        public IEnumerable<Currency_DTO> Currencies => _csc.Items.Values;

        public Currency_DTO Currency(long id) => _csc.Item(id);
    }
}
