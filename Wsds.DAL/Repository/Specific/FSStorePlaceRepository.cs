using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Specific
{
    public class FSStorePlaceRepository : IStorePlaceRepository
    {
        ICacheService<StorePlace_DTO> _csStorePlace;
        ICacheService<Store_DTO> _csStore;

        public FSStorePlaceRepository(ICacheService<StorePlace_DTO> csStorePlace, ICacheService<Store_DTO> csStore)
        {
            _csStorePlace = csStorePlace;
            _csStore = csStore;
        }
        public IEnumerable<ProductStorePlace_DTO> ProductStorePlaces => throw new NotImplementedException();

        public IEnumerable<StorePlace_DTO> StorePlaces => _csStorePlace.Items.Values;

        public IEnumerable<ProductStorePlace_DTO> GetProductSPByQuotId(long idQuot)
        {
            var cnfg = EntityConfigDictionary.GetConfig("product_store_place");
            var prov = new EntityProvider<ProductStorePlace_DTO>(cnfg);
            return prov.GetItems("id_Quotation_Product = :idQuot and qty>0", new OracleParameter("a", idQuot));
        }

        public IEnumerable<Store_DTO> Stores => _csStore.Items.Values;

        public Store_DTO GetStore(long id) => _csStore.Item(id);

        public ProductStorePlace_DTO ProductStorePlace(long id)
        {
            throw new NotImplementedException();
        }

        public StorePlace_DTO StorePlace(long id) => _csStorePlace.Item(id);

        public IEnumerable<Store_DTO> GetFavoriteStores(long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("stores");
            var prov = new EntityProvider<Store_DTO>(cnfg);
            var favStores = prov.GetItems("t.id in (SELECT fs.id_store_places FROM favorite_stores fs where fs.id_client = :id)", new OracleParameter("id", idClient));
            return favStores;
        }


        public long AddFavoriteStore(long idStore, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("favorite_stores");
            var prov = new EntityProvider<FavoriteStore_DTO>(cnfg);
            var favStore = new FavoriteStore_DTO
            {
                idClient = idClient,
                idStore = idStore
            };
            var stores = prov.GetItems("t.id_client = :clientId and t.id_store_places = :storeId", new OracleParameter("clientId", idClient), new OracleParameter("storeId", idStore));
            if (stores.Count() == 0)
            {
                var addedStore = prov.InsertItem(favStore);
                return addedStore.idStore;
            }
            return 0;
        }

        public long DeleteFavoriteStore(long idStore, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("favorite_stores");
            var prov = new EntityProvider<FavoriteStore_DTO>(cnfg);
            var stores = prov.GetItems("t.id_client = :clientId and t.id_store_places = :storeId", new OracleParameter("clientId", idClient), new OracleParameter("storeId", idStore));
            if (stores.Count() > 0)
            {
                FavoriteStore_DTO store = stores.FirstOrDefault();
                prov.DeleteItem((long)store.id);
                return store.idStore;
            }
            return 0;
        }
    }
}
