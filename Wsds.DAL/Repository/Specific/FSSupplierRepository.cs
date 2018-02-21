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
    public class FSSupplierRepository : ISupplierRepository
    {
        private ICacheService<Supplier_DTO> _css;

        public FSSupplierRepository(ICacheService<Supplier_DTO> css) => _css = css;
        public IEnumerable<Supplier_DTO> Suppliers => _css.Items.Values;

        public Supplier_DTO Supplier(long id) => _css.Item(id);

    }
}
