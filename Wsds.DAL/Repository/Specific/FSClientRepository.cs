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

    }
}
