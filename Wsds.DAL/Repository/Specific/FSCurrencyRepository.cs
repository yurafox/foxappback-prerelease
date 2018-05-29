using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSCurrencyRepository : ICurrencyRepository
    {
        private ICacheService<Currency_DTO> _csc;
        private ICacheService<CurrencyRate_DTO> _crc;

        public FSCurrencyRepository(ICacheService<Currency_DTO> csc, ICacheService<CurrencyRate_DTO> crc)
        {
            _csc = csc;
            _crc = crc;
        }

        public IEnumerable<Currency_DTO> Currencies => _csc.Items.Values;
        public IEnumerable<CurrencyRate_DTO> CurrencyRate => _crc.Items.Values; 
        public Currency_DTO Currency(long id) => _csc.Item(id);

        
    }
}
