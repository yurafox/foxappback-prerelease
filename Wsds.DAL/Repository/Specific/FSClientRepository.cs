using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Infrastructure;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;

namespace Wsds.DAL.Repository.Specific
{
    public class FSClientRepository : IClientRepository
    {
        private readonly ICacheService<Client_DTO> _csClient;
        private readonly IConfiguration _config;

        public FSClientRepository(ICacheService<Client_DTO> csClient, IConfiguration config) {
            _csClient = csClient;
            _config = config;
        }
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

        public PersonInfo_DTO CreatePerson(PersonInfo_DTO item)
        {
            var persCnfg = EntityConfigDictionary.GetConfig("person_info");
            var prov = new EntityProvider<PersonInfo_DTO>(persCnfg);
            return prov.InsertItem(item);
        }

        public PersonInfo_DTO UpdatePerson(PersonInfo_DTO item)
        {
            var persCnfg = EntityConfigDictionary.GetConfig("person_info");
            var prov = new EntityProvider<PersonInfo_DTO>(persCnfg);
            return prov.UpdateItem(item);
        }

        public object GetClientBonusesInfo(long idClient)
        {
            object res = null;
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(UrlConstants.GetBonusInfoUrl + "/" + idClient.ToString()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    var responseObj = (JObject)JsonConvert.DeserializeObject(responseString);

                    return  new {
                                 bonusLimit = responseObj.GetValue("regular"),
                                 actionBonusLimit = responseObj.GetValue("special")
                                };
                }
            }
            return res;
        }

        public IEnumerable<object> GetClientBonusesExpireInfo(long idClient)
        {
            List<object> res = new List<object>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(UrlConstants.GetBonusesExpireInfoUrl + "/" + idClient.ToString()).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    JArray responseObj = (JArray)JsonConvert.DeserializeObject(responseString);

                    foreach (JObject obj in responseObj) {
                        res.Add(new {
                                        bonus = obj.Property("quantity").Value,
                                        dueDate = obj.Property("expiryDate").Value,
                                        type = obj.Property("type").Value
                        });
                    }
                }
            }
            return res;
        }

        public void LogProductView(long idProduct, string viewParams)
        {
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin " +
                                               "insert into PRODUCT_VIEW_HISTORY " +
                                               "(id, id_client, id_product, date_of_view, view_params) " +
                                               "values " +
                                               "(seq_PRODUCT_VIEW_HISTORY.nextval, :idClient, :idProduct, sysdate, :viewParams); " +
                                               "commit; end;", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("idClient", 100)); //TODO я
                    cmd.Parameters.Add(new OracleParameter("idProduct", idProduct));
                    cmd.Parameters.Add(new OracleParameter("viewParams", viewParams));
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            };
        }

        public ClientAddress_DTO ClientAddress(long id)
        {
            var caCnfg = EntityConfigDictionary.GetConfig("client_address");
            var prov = new EntityProvider<ClientAddress_DTO>(caCnfg);
            return prov.GetItem(id);
        }

        public IEnumerable<ClientAddress_DTO> GetClientAddressesByClientId(long id)
        {
            var caCnfg = EntityConfigDictionary.GetConfig("client_address");
            var prov = new EntityProvider<ClientAddress_DTO>(caCnfg);
            return prov.GetItems("id_client =:id", new OracleParameter("id", id));
        }

        public ClientAddress_DTO CreateClientAddress(ClientAddress_DTO item)
        {
            var caCnfg = EntityConfigDictionary.GetConfig("client_address");
            var prov = new EntityProvider<ClientAddress_DTO>(caCnfg);
            item.idClient = 100; //TODO я

            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin " +
                                               "update CLIENT_ADDRESS " +
                                               "set is_primary = null " +
                                               "where id_client = :idClient ;" +
                                               "end;", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("idClient", item.idClient));
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            };

            return prov.InsertItem(item);

        }

        public ClientAddress_DTO UpdateClientAddress(ClientAddress_DTO item)
        {
            var caCnfg = EntityConfigDictionary.GetConfig("client_address");
            var prov = new EntityProvider<ClientAddress_DTO>(caCnfg);
            return prov.UpdateItem(item);
        }

        public void DeleteClientAddress(long id)
        {
            var caCnfg = EntityConfigDictionary.GetConfig("client_address");
            var prov = new EntityProvider<ClientAddress_DTO>(caCnfg);

            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin " +
                                               "update CLIENT_ADDRESS " +
                                               "set is_deleted = 1 " +
                                               "where id = :id ;" +
                                               "end;", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("idClient", id));
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            };
        }
    }
}
