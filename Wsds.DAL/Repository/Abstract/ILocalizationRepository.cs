using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ILocalizationRepository
    {
        IEnumerable<Lang_DTO> Langs { get; }
        Lang_DTO Lang(long id);

    }
}
