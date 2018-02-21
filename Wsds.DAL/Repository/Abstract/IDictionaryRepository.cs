using System;
using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    // Репозиторий для сущностей-справочников (товары, группы, производители, валюты)
    public interface IDictionaryRepository
    {
        # region Product
        IEnumerable<Product_DTO> Products { get; }
        Product_DTO Product(long id);
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
        IEnumerable<Currency_DTO> Currencies { get; }
        Currency_DTO Currency(long id);
        IEnumerable<Currency> CurAscending { get; }
        #endregion

        IEnumerable<Manufacturer_DTO> Manufacturers { get; }
        Manufacturer_DTO Manufacturer(long id);

        IEnumerable<Quotation_Product_DTO> QuotationProducts { get; }
        Quotation_Product_DTO QuotationProduct(long id);

        IEnumerable<Supplier_DTO> Suppliers { get; }
        Supplier_DTO Supplier(long id);


    }
}
