using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using Wsds.DAL.Entities;
using Wsds.DAL.ORM;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSBrandRepository:IBrandRepository
    {
        private readonly FoxStoreDBContext _context;
        private readonly ICacheService<Product> _product;

        public IEnumerable<Manufacturer> GetAll => _product.Items
            .Select(p => p.MANUFACTURER)
            .Distinct((m, m1) => m.ID==m1.ID);

        public FSBrandRepository(FoxStoreDBContext context, ICacheService<Product> product)
        {
            _context = context;
            _product = product;
        }


        public Manufacturer GetBrandById(int id)
        {
            return _product.Items
                .Select(p => p.MANUFACTURER)
                .FirstOrDefault(m => m.ID == id);
        }

        public IEnumerable<Manufacturer> GetBrandByCategory(int categoryId)
        {
            var brands = _product.Items
                .Where(p => p.PRODUCTS_IN_GROUPS.Any(gr => gr.ID_GROUP == categoryId))
                .Select(p => p.MANUFACTURER)
                .GroupBy(m => new {m.ID,m.NAME,m.URL}).ToList();


            var brResult = brands.Select(br=>new Manufacturer()
            {
                ID = br.Key.ID,
                NAME = br.Key.NAME,
                URL = br.Key.URL,
                Count = br.Count()
            });

            return brResult;
        }
    }
}
