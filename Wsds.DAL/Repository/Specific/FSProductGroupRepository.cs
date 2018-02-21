using System.Collections.Generic;
using System.Linq;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSProductGroupRepository:IProductGroupRepository
    {
        private ICacheService<Product_Groups_DTO> _pg;
        public FSProductGroupRepository(ICacheService<Product_Groups_DTO> pg)
        {
            _pg = pg;
        }

        public IEnumerable<Product_Groups_DTO> ProductGroups => _pg.Items.Values.ToList();

        public Product_Groups_DTO GetProductGroupById(long id)
        {
           return _pg.Item(id);
        }
    }
}
