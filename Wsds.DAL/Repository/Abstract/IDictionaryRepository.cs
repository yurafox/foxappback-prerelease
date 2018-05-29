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


        #region Currency
        IEnumerable<Currency_DTO> Currencies { get; }
        Currency_DTO Currency(long id);
        #endregion

        IEnumerable<Manufacturer_DTO> Manufacturers { get; }
        Manufacturer_DTO Manufacturer(long id);

        IEnumerable<Quotation_Product_DTO> QuotationProducts { get; }
        Quotation_Product_DTO QuotationProduct(long id);

        IEnumerable<Supplier_DTO> Suppliers { get; }
        Supplier_DTO Supplier(long id);


    }
}
