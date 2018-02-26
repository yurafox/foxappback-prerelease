using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository
{
    public interface IStorePlaceRepository
    {
        IEnumerable<ProductStorePlace_DTO> ProductStorePlaces { get; }
        ProductStorePlace_DTO ProductStorePlace(long id);
        IEnumerable<ProductStorePlace_DTO> GetProductSPByQuotId(long idQuot);

        IEnumerable<StorePlace_DTO> StorePlaces { get; }
        StorePlace_DTO StorePlace(long id); 
    }
}
