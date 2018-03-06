using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;

namespace Wsds.DAL.Repository.Specific
{
    public class FSStorePlaceRepository : IStorePlaceRepository
    {
        ICacheService<StorePlace_DTO> _csStorePlace;

        public FSStorePlaceRepository(ICacheService<StorePlace_DTO> csStorePlace) => _csStorePlace = csStorePlace;
        public IEnumerable<ProductStorePlace_DTO> ProductStorePlaces => throw new NotImplementedException();

        public IEnumerable<StorePlace_DTO> StorePlaces => _csStorePlace.Items.Values;

        public IEnumerable<ProductStorePlace_DTO> GetProductSPByQuotId(long idQuot)
        {
            var cnfg = EntityConfigDictionary.GetConfig("product_store_place");
            var prov = new EntityProvider<ProductStorePlace_DTO>(cnfg);
            return prov.GetItems("id_Quotation_Product = :idQuot and qty>0", new OracleParameter("a", idQuot));
        }

        public IEnumerable<Store_DTO> GetStores()
        {
            var cnfg = EntityConfigDictionary.GetConfig("stores");
            var prov = new EntityProvider<Store_DTO>(cnfg);
            return prov.GetItems();
        }

        public ProductStorePlace_DTO ProductStorePlace(long id)
        {
            throw new NotImplementedException();
        }

        public StorePlace_DTO StorePlace(long id) => _csStorePlace.Item(id);
    }
}
