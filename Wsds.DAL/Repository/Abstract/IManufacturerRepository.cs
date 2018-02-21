using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IManufacturerRepository
    {
        IEnumerable<Manufacturer_DTO> Manufacturers { get; }
        Manufacturer_DTO Manufacturer(long id);
    }
}
