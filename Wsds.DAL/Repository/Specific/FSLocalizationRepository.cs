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
    public class FSLocalizationRepository : ILocalizationRepository
    {
        ICacheService<Lang_DTO> _csl;
        public FSLocalizationRepository(ICacheService<Lang_DTO> csl) => _csl = csl;
        public IEnumerable<Lang_DTO> Langs => _csl.Items.Values;

        public Lang_DTO Lang(long id) => _csl.Item(id);
    }
}
