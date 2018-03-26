using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface IClientMessageRepository
    {
        ClientMessage_DTO SaveClientMessage(ClientMessage_DTO message, long idClient);
    }
}
