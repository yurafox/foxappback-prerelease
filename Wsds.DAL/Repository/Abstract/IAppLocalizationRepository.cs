using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IAppLocalizationRepository
    {
        IEnumerable<Localization_DTO> GetFrontLocale();
        IEnumerable<Localization_DTO> GetBackLocale();
    }
}
