using System;
using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    // Репозиторий для сущностей-справочников (товары, группы, производители, валюты)
    public interface IDictionaryRepository
    {
        # region Product
        IEnumerable<Product> Products { get; }
        Product Product(int id);
        #endregion

        # region ProductCache
        IEnumerable<Product> ProductsCache { get; }
        Product ProductCache(int id);
        #endregion

        #region ProductsGroupsCache
        IEnumerable<Product_Group> ProductsGroupsCache { get; }
        IEnumerable<Product_Group> ProductGroupsByFilterCache(Func<Product_Group,bool> filter);
        Product_Group ProductGroupSingleByFilterCache(Func<Product_Group,bool> singleFilter);
        #endregion

        #region ProductGroup
        IEnumerable<Product_Group> ProductGroups { get; }
        //ProductGroup ProductGroup(int id);
        #endregion

        #region Currency
        IEnumerable<Currency> Currencies { get; }
        Currency Currency(int id);
        IEnumerable<Currency> CurAscending { get; }
        #endregion
    }
}
