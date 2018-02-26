using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ICartRepository
    {
        IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByUserId(long userId);
        IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByClietId(long clientId);
        ClientOrderProduct_DTO UpdateCartProduct(ClientOrderProduct_DTO item);
        ClientOrderProduct_DTO InsertCartProduct(ClientOrderProduct_DTO item);
        void DeleteCartProduct(long id);
    }
}
