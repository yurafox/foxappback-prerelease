using System.Collections.Generic;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IStorePlaceRepository
    {
        IEnumerable<ProductStorePlace_DTO> ProductStorePlaces { get; }
        ProductStorePlace_DTO ProductStorePlace(long id);
        IEnumerable<ProductStorePlace_DTO> GetProductSPByQuotId(long idQuot);

        IEnumerable<StorePlace_DTO> StorePlaces { get; }
        StorePlace_DTO StorePlace(long id);

        IEnumerable<Store_DTO> Stores { get; }
        Store_DTO GetStore(long id);

        IEnumerable<Store_DTO> GetFavoriteStores(long idClient);
        long AddFavoriteStore(long idStore, long idClient);
        long DeleteFavoriteStore(long idStore, long idClient);
    }
}
