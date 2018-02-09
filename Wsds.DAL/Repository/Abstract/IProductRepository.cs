using System.Collections.Generic;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    interface IProductRepository
    {
        IEnumerable<Product> Products { get; }
        IEnumerable<Product_Group> ProductGroups { get; }
    }
}
