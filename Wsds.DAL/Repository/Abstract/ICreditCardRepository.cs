using System.Collections.Generic;
using Wsds.DAL.Entities.DTO;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ICreditCardRepository
    {
        IEnumerable<CreditCard_DTO> GetCreditCardsByClientId(long clientId);
    }
}
