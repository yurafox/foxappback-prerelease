﻿using System;
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
        ClientOrderProduct_DTO UpdateCartProduct(ClientOrderProduct_DTO item,long clientId);
        ClientOrderProduct_DTO InsertCartProduct(ClientOrderProduct_DTO item,long clientId,long currency);
        ClientOrder_DTO GetOrCreateClientDraftOrder(long clientId,long currencyId);
        ClientOrder_DTO SaveClientOrder(ClientOrder_DTO order,long clientId);
        void DeleteCartProduct(long id,long clientId);
        IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByOrderId(long orderId);
        IEnumerable<ClientOrder_DTO> GetClientOrders(long clientId);
        IEnumerable<CalculateCartResponse> CalculateCart(CalculateCartRequest cartObj,long card);
        PostOrderResponse PostOrder(ClientOrder_DTO order);
        IEnumerable<ClientOrderProductsByDate_DTO> GetOrderProductsByDate(string datesRange);
        ClientOrder_DTO GetClientOrder(long orderId);
    }
}
