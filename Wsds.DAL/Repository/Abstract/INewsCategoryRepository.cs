using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface INewsCategoryRepository
    {
        IEnumerable<NewsCategory_DTO> NewsCategory { get; }
    }
}
