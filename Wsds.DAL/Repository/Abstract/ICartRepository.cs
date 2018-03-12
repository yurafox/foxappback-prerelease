using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;

namespace Wsds.DAL.Repository.Abstract
{
    public interface ICartRepository
    {
        IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByUserId(long userId);
        IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByClietId(long clientId);
        ClientOrderProduct_DTO UpdateCartProduct(ClientOrderProduct_DTO item);
        ClientOrderProduct_DTO InsertCartProduct(ClientOrderProduct_DTO item);
        ClientOrder_DTO GetOrCreateClientDraftOrder();
        ClientOrder_DTO SaveClientOrder(ClientOrder_DTO order);
        void DeleteCartProduct(long id);
        IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByOrderId(long orderId);
        IEnumerable<ClientOrder_DTO> GetClientOrders();
        IEnumerable<CalculateCartResponse> CalculateCart(CalculateCartRequest cartObj);
        PostOrderResponse PostOrder(ClientOrder_DTO order);
    }
}
