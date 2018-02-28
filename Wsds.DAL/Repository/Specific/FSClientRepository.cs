using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Infrastructure;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using System.Runtime.Serialization.Json;

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

        public PersonInfo_DTO GetPersonById(long idPerson)
        {
            var persCnfg = EntityConfigDictionary.GetConfig("person_info");
            var prov = new EntityProvider<PersonInfo_DTO>(persCnfg);
            return prov.GetItem(idPerson);
        }

        public async Task<object> GetClientBonusesInfoAsync(long idClient)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
            var streamTask = client.GetStreamAsync(UrlConstants.GetBonusInfoUrl);
            var bonusInfo = await streamTask;
            return bonusInfo;
        }
    }
}
