using Wsds.DAL.Entities.DTO;
using System.Collections.Generic;
using Wsds.DAL.Repository.Abstract;
using Wsds.DAL.Providers;
using Oracle.ManagedDataAccess.Client;

namespace Wsds.DAL.Repository.Specific
{
    public class FSCreditCardRepository : ICreditCardRepository
    {
        public IEnumerable<CreditCard_DTO> GetCreditCardsByClientId(long clientId)
        {
            var creditCardConfig = EntityConfigDictionary.GetConfig("client_credit_cards");
            var provider = new EntityProvider<CreditCard_DTO>(creditCardConfig);

            return provider.GetItems("t.id_client = :clientId", new OracleParameter("clientId", clientId));
        }
    }
}
