using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using System.Linq;

namespace Wsds.DAL.Repository.Specific
{
    public class FSAppLocalizationRepository: IAppLocalizationRepository
    {
        private ICacheService<Localization_DTO> _csl;

        public FSAppLocalizationRepository(ICacheService<Localization_DTO> csl)
        {
            _csl = csl;
        }

        public IEnumerable<Localization_DTO> GetFrontLocale()
        {
            var cnfg = EntityConfigDictionary.GetConfig("global_localization");
            var prov = new EntityProvider<Localization_DTO>(cnfg);
            var loc = prov.GetItems("t.is_front_or_back=1");
            if (loc != null && loc.Count() > 0) return loc;
            return null;
        }

        public IEnumerable<Localization_DTO> GetBackLocale()
        {
            var cnfg = EntityConfigDictionary.GetConfig("global_localization");
            var prov = new EntityProvider<Localization_DTO>(cnfg);
            var loc = prov.GetItems("t.is_front_or_back=0");
            if (loc != null && loc.Count() > 0) return loc;
            return null;
        }

        public string GetBackLocaleString(string compName, string tagName)
        {
            var locales = GetBackLocale();
            var localeRow = locales.FirstOrDefault(l=>l.componentName==compName && l.tagName== tagName);
            return localeRow?.text;
        }
    }
}
