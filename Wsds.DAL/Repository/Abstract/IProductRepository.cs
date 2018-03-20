using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IProductRepository
    {
        IEnumerable<Product_DTO> Products { get; }
        Product_DTO Product(long id);
        string GetProductDescription(long id);
        IEnumerable<string> GetProductImages(long id);
        IEnumerable<Product_DTO> SearchProducts(string srchString);
        IEnumerable<Product_DTO> SearchProductsInCache(string srchString);
    }
}
