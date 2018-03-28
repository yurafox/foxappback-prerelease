using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Oracle.ManagedDataAccess.Client;
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

        public IEnumerable<Localization_DTO> GetLocale()
        {
            var cnfg = EntityConfigDictionary.GetConfig("global_localization");
            var prov = new EntityProvider<Localization_DTO>(cnfg);
            var loc = prov.GetItems();
            if (loc != null && loc.Count() > 0) return loc;
            return null;
        }

        //public Localization_DTO SaveLocalization(Localization_DTO localization)
        //{
        //    var cnfg = EntityConfigDictionary.GetConfig("global_localization");
        //    var prov = new EntityProvider<Localization_DTO>(cnfg);
        //    localization.frontOrBack = 1;
        //    if (localization.componentName != null && localization.tagName != null && localization.text != null)
        //    {
        //        var loc = prov.GetItems("t.component_name = :compName and t.tag_name = :tagName and t.id_lang = :lang and t.locale_text = :text", new OracleParameter("compName", localization.componentName), new OracleParameter("compName", localization.tagName), new OracleParameter("lang", localization.lang), new OracleParameter("text", localization.text));
        //        if (loc != null && loc.Count() > 0) return null;
        //        return prov.InsertItem(localization);
        //    }
        //    return null;
        //}
    }
}
