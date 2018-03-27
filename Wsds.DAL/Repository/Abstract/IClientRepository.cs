using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IClientRepository
    {
        IEnumerable<Client_DTO> Clients { get; }
        Client_DTO Client(long id);
        IEnumerable<Client_DTO> GetClientByUserID(long userId);
        IEnumerable<Client_DTO> GetClientByEmail(string email);
        PersonInfo_DTO GetPersonById(long idPerson);
        PersonInfo_DTO CreatePerson(PersonInfo_DTO item);
        PersonInfo_DTO UpdatePerson(PersonInfo_DTO item);

        object GetClientBonusesInfo(long idClient);
        IEnumerable<object> GetClientBonusesExpireInfo(long idClient);
        void LogProductView(long idProduct, string viewParams);
        ClientAddress_DTO ClientAddress(long id);
        IEnumerable<ClientAddress_DTO> GetClientAddressesByClientId(long id);
        ClientAddress_DTO CreateClientAddress(ClientAddress_DTO item);
        ClientAddress_DTO UpdateClientAddress(ClientAddress_DTO item);
        void DeleteClientAddress(long id);
        Client_DTO GetClientByPhone(string phone);
        IEnumerable<Client_DTO> GetClientsByPhone(string phone);
        IEnumerable<StorePlace_DTO> GetFavoriteStore(long clientId);
        Client_DTO CreateOrUpdateClient(Client_DTO client);
        AppKeys_DTO GetApplicationKeyByClientId(long idClient);
        AppKeys_DTO CreateApplicationKey(AppKeys_DTO key);
    }
}
