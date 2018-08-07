﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
using Dapper;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Wsds.DAL.Entities.DTO;
using System.Globalization;

namespace Wsds.DAL.Repository.Specific
{
    public class FSClientRepository : IClientRepository
    {
        private readonly ICacheService<Client_DTO> _csClient;
        private readonly IConfiguration _config;
        private IProductRepository _prodRepo;

        public FSClientRepository(ICacheService<Client_DTO> csClient, IConfiguration config, IProductRepository prodRepo) {
            _csClient = csClient;
            _config = config;
            _prodRepo = prodRepo;
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

        public object GetClientBonusesInfo(long card)
        {
            using (var client = new HttpClient()) {
                client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(UrlConstants.GetBonusInfoUrl + "/" + card).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    var responseObj = (JObject)JsonConvert.DeserializeObject(responseString);

                    return new
                    {
                        bonusLimit = responseObj.GetValue("regular"),
                        actionBonusLimit = responseObj.GetValue("special")
                    };
                }
                else
                {
                    return null;
                }

            }
        }

        public IEnumerable<object> GetClientBonusesExpireInfo(long card)
        {
            List<object> res = new List<object>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(UrlConstants.GetBonusesExpireInfoUrl + "/" + card).Result;
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

        public void LogProductView(long idProduct, string viewParams, long clientId)
        {
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))

            using (var cmd = new OracleCommand("begin data_utils.StoreProductViewHistory(clientId =>:idClient, productId => :idProduct, viewParams => :viewParams); end;", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("idClient", clientId));
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

            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin " +
                                               "update CLIENT_ADDRESS " +
                                               "set is_primary = null " +
                                               "where id_client = :idClient ;" +
                                               "commit; end;", con))
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
            var idClient = item.idClient.Value;
            var caCnfg = EntityConfigDictionary.GetConfig("client_address");
            var prov = new EntityProvider<ClientAddress_DTO>(caCnfg);

            if ((bool)item.isPrimary) {
                foreach (var i in prov.GetItems("t.id_client = :idClient and nvl(is_deleted,0)=0", 
                                                new OracleParameter("idClient", idClient)
                                                )
                        )
                {
                    if (i.id != item.id) {
                        i.isPrimary = false;
                        prov.UpdateItem(i);
                    }
                }
            }
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
                                               "commit; end;", con))
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
        public IEnumerable<StorePlace_DTO> GetFavoriteStore(long clientId)
        {
            var clCnfg = EntityConfigDictionary.GetConfig("store_place");
            var prov = new EntityProvider<StorePlace_DTO>(clCnfg);
            return prov.GetItems("id in (select f.id_store_places from favorite_stores f " +
                                 "where f.id_client = :id_client)", new OracleParameter("id_client", clientId));
        }


        public IEnumerable<Client_DTO> GetClientsByPhone(string phone)
        {
            var clientList = new List<Client_DTO>();
            string res = "";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin :result := app_core.getclientbyphone(:phonenum); end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 2000;
                    cmd.Parameters.Add(new OracleParameter("phonenum", phone));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();
                }
                finally
                {
                    con.Close();
                }
            };

            if (String.IsNullOrEmpty(res))
                return null;

            var client = JsonConvert.DeserializeObject<Client_DTO>(res);
          

            if (client?.id != null)
            {
                var applicationKey = GetApplicationKeyByClientId(client.id.Value);
                client.appKey = applicationKey?.key;
                clientList.Add(client);
            }

            return clientList;
        }


        public Client_DTO GetClientById(long id)
        {
            var clientList = new List<Client_DTO>();
            string res = "";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin :result := app_core.getClientById(:id); end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 2000;
                    cmd.Parameters.Add(new OracleParameter("id", id));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();
                }
                finally
                {
                    con.Close();
                }
            };

            if (String.IsNullOrEmpty(res))
                return null;

            var client = JsonConvert.DeserializeObject<Client_DTO>(res);


            if (client?.id != null)
            {
                var applicationKey = GetApplicationKeyByClientId(client.id.Value);
                client.appKey = applicationKey?.key;
            }

            return client;
        }

        public Client_DTO CreateOrUpdateClient(Client_DTO client)
        {
            if (client == null) return null;

            string res = "";
            var connString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(connString))
            using (var cmd = new OracleCommand("begin :result:= app_core.createorupdateclient(:pphone,:pcard,:pemail,:pfname," +
                                               ":plname,:pid_currency,:pid_lang);end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 2000;
                    cmd.Parameters.Add(new OracleParameter("pphone", client.phone));
                    cmd.Parameters.Add(new OracleParameter("pcard", client.barcode ?? (object) DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("pemail", client.email));
                    cmd.Parameters.Add(new OracleParameter("pfname", client.fname));
                    cmd.Parameters.Add(new OracleParameter("plname", client.lname ?? (object) DBNull.Value));
                    cmd.Parameters.Add(new OracleParameter("pid_currency", client.id_currency));
                    cmd.Parameters.Add(new OracleParameter("pid_lang", client.id_lang));

                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();
                }
                catch (OracleException ex)
                {
                    //TODO: create handler logic after discussion
                }
                finally
                {
                    con.Close();
                }
            };
                return JsonConvert.DeserializeObject<Client_DTO>(res);
        }

        public AppKeys_DTO GetApplicationKeyByClientId(long idClient)
        {
            var akCnfg = EntityConfigDictionary.GetConfig("application_keys");
            var prov = new EntityProvider<AppKeys_DTO>(akCnfg);
            return prov.GetItems("id_client =:idClient and k.date_end >= sysdate and k.date_start <=sysdate",
                new OracleParameter("id_client", idClient)).FirstOrDefault();
        }

        public AppKeys_DTO CreateApplicationKey(AppKeys_DTO key)
        {
            var akCnfg = EntityConfigDictionary.GetConfig("application_keys");
            var prov = new EntityProvider<AppKeys_DTO>(akCnfg);
            return prov.InsertItem(key);
        }

        public Client_DTO GetClientByPhone(string phone)
        {
            return GetClientsByPhone(phone).FirstOrDefault();
        }


        public IEnumerable<ClientOrderDatesRange_DTO> GetClientOrderDatesRanges(DateTime clientCreatedDate)
        {
            List<ClientOrderDatesRange_DTO> res = new List<ClientOrderDatesRange_DTO> {
                new ClientOrderDatesRange_DTO { key = "30d", displayName = "Last 30 days" }, //TODO localization
                new ClientOrderDatesRange_DTO { key = "6m", displayName = "Last 6 months" , isDefault = true}  //TODO localization
            };

            int yearReg = clientCreatedDate.Year;
            int yearNow = DateTime.Now.Year;

            for (var i = yearNow; i >= yearReg; i--) {
                res.Add(new ClientOrderDatesRange_DTO { key = i.ToString(), displayName = i.ToString() });
            }

            return res;
        }

        public void CallMe(string phone) {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(
                                new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.GetAsync(String.Format(UrlConstants.CallMeServiceUrl, phone.Substring(2,10))).Result;

            }
        }

        public IEnumerable<Product_DTO> GetProductsView(long clientId)
        {
            var ConnString = _config.GetConnectionString("MainDataConnection");

            List<Product_DTO> res = new List<Product_DTO>();
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("select id_product from PRODUCT_VIEW_HISTORY t " +
                                               "where t.id_client = :idClient " +
                                               "order by t.date_of_view desc " +
                                               "fetch first 20 rows only", con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("idClient", clientId));
 
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var productId = (long)reader["id_product"];

                        Product_DTO product = _prodRepo.Product(productId);
                        res.Add(product);
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

    }
}
