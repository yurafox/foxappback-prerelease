using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface INoveltyRepository
    {
        IEnumerable<Novelty_DTO> GetNovelties();
        Novelty_DTO GetNoveltyById(long id);

        IEnumerable<NoveltyDetails_DTO> GetNoveltyDetailsByNoveltyId(long id);
    }
}
