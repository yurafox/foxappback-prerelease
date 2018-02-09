using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IBrandRepository
    {
        IEnumerable<Manufacturer> GetAll { get; }
        Manufacturer GetBrandById(int id);
        IEnumerable<Manufacturer> GetBrandByCategory(int categoryId);
    }
}
