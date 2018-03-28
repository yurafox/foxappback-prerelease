using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IAppLocalizationRepository
    {
        IEnumerable<Localization_DTO> GetLocale();

        //Localization_DTO SaveLocalization(Localization_DTO localization);
    }
}
