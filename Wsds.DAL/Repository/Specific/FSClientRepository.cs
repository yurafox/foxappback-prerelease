using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSClientRepository : IClientRepository
    {
        private ICacheService<Client_DTO> _csClient;

        public FSClientRepository(ICacheService<Client_DTO> csClient) => _csClient = csClient;
        public IEnumerable<Client_DTO> Clients => _csClient.Items.Values;

        public Client_DTO Client(long id) => _csClient.Item(id);

        public IEnumerable<Client_DTO> GetClientByUserID(long userId)
        {
            var clCnfg = EntityConfigDictionary.GetConfig("client");
            var prov = new EntityProvider<Client_DTO>(clCnfg);
            return prov.GetItems("user_id = :id", new OracleParameter("id", userId));
        }

        public IEnumerable<Client_DTO> GetClientByEmail(string email)
        {
            var clCnfg = EntityConfigDictionary.GetConfig("client");
            var prov = new EntityProvider<Client_DTO>(clCnfg);
            return prov.GetItems("email = :email", new OracleParameter("email", email));
        }

        public IEnumerable<StorePlace_DTO> GetFavoriteStore(long clientId)
        {
            var clCnfg = EntityConfigDictionary.GetConfig("store_place");
            var prov = new EntityProvider<StorePlace_DTO>(clCnfg);
            return prov.GetItems("id in (select f.id_store_places from favorite_stores f " +
                                 "where f.id_client = :email)", new OracleParameter("id_client", clientId));
        }

        public Client_DTO GetClientByPhone(string phone)
        {
            var clCnfg = EntityConfigDictionary.GetConfig("client");
            var prov = new EntityProvider<Client_DTO>(clCnfg);
            var data = prov.GetItems("phone = :phone", new OracleParameter("phone", phone));
            return data?.FirstOrDefault();
        }
    }
}
