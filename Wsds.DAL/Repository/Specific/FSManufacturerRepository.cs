using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSManufacturerRepository : IManufacturerRepository
    {
        private ICacheService<Manufacturer_DTO> _csm;

        public FSManufacturerRepository(ICacheService<Manufacturer_DTO> csm) => _csm = csm;

        public IEnumerable<Manufacturer_DTO> Manufacturers => _csm.Items.Values;

        public Manufacturer_DTO Manufacturer(long id) => _csm.Item(id);
    }

}
