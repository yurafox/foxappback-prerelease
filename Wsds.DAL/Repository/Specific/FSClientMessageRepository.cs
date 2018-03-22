using Wsds.DAL.Entities;
using Wsds.DAL.Entities.DTO;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSClientMessageRepository: IClientMessageRepository
    {
        public ClientMessage_DTO SaveClientMessage(ClientMessage_DTO message, long idClient)
        {
            var cnfg = EntityConfigDictionary.GetConfig("client_messages");
            var prov = new EntityProvider<ClientMessage_DTO>(cnfg);
            var _message = new ClientMessage_DTO
            {
                idClient = idClient,
                messageDate = message.messageDate,
                messageText = message.messageText,
                isAnswered = 0
            };
            if (_message != null)
            {
                return prov.InsertItem(_message);
            }
            else return null;
        }
    }
}
