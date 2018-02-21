using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSProductRepository : IProductRepository
    {
        private ICacheService<Product_DTO> _csp; 
        public FSProductRepository(ICacheService<Product_DTO> csp) => _csp = csp;

        public IEnumerable<Product_DTO> Products => _csp.Items.Values;

        public Product_DTO Product(long id) => _csp.Item(id);
    }
}
