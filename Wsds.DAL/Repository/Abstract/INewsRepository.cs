using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface INewsRepository
    {
        IEnumerable<News_DTO> News { get; }
        string GetNewsDescription(long id);
        IEnumerable<News_DTO> SearchNewsInCache(int categoryId);
    }
}
