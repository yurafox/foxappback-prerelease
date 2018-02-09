using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using FoxStoreDBContext = Wsds.DAL.ORM.FoxStoreDBContext;

namespace Wsds.DAL.Repository.Specific
{
    public class FSDictionaryRepository : IDictionaryRepository
    {
        private FoxStoreDBContext _context;
        private ICacheService<Product> _csp;
        private ICacheService<Product_Group> _csg;

        public FSDictionaryRepository(FoxStoreDBContext dbContext, 
                                      ICacheService<Product> csp,
                                      ICacheService<Product_Group> csg) {
            _context = dbContext;
            _csp = csp;
            _csg = csg;
        }


        public IEnumerable<Currency> Currencies
        {
            get
            {
                return _context.CURRENCIES;
            }
        }
        public Currency Currency(int id)
        {
            return _context.CURRENCIES.FirstOrDefault(c => c.ID == id);
        }

        public IEnumerable<Currency> CurAscending => _context.CURRENCIES.OrderBy(c => c.SHORT_NAME);

        public IEnumerable<Product_Group> ProductGroupsByFilterCache(Func<Product_Group, bool> filter)
        {
            return _csg.Items.Where(filter);
        }

        public Product_Group ProductGroupSingleByFilterCache(Func<Product_Group, bool> singleFilter)
        {
            return _csg.Items.FirstOrDefault(singleFilter);
        }

        public IEnumerable<Product_Group> ProductGroups => _context.PRODUCT_GROUPS;

        public Product Product(int id)
        {
            return _context.PRODUCTS.FirstOrDefault(p => p.ID == id);
        }

        
        public IEnumerable<Product> Products
        {
            get
            {
                return _context.PRODUCTS;
            }
        }


        public Product ProductCache(int id)
        {
            return _csp.Items.FirstOrDefault(p => p.ID == id);
        }

        public IEnumerable<Product> ProductsCache
        {
            get
            {
                return _csp.Items;
            }
        }

        public IEnumerable<Product_Group> ProductsGroupsCache => _csg.Items;

    }
}