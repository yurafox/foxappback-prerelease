﻿using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSCartRepository : ICartRepository
    {
        private IClientRepository _clRepo;
        public FSCartRepository(IClientRepository clRepo) => _clRepo = clRepo;

        public IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByUserId(long userId)
        {
            var client = _clRepo.GetClientByUserID(userId);
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);

            return prov.GetItems("o.id_client = :idclient", new OracleParameter("idclient", client.FirstOrDefault().id));
        }

        public IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByClietId(long clientId) {
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);

            return prov.GetItems("o.id_client = :idclient", new OracleParameter("idclient", clientId));
        }

        public ClientOrderProduct_DTO UpdateCartProduct(ClientOrderProduct_DTO item)
        {
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);
            return prov.UpdateItem(item); 
        }

        public ClientOrderProduct_DTO InsertCartProduct(ClientOrderProduct_DTO item)
        {
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);
            item.idOrder = GetOrCreateClientDraftOrderID();
            return prov.InsertItem(item);
        }

        public void DeleteCartProduct(long id)
        {
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);
            prov.DeleteItem(id);
        }

        public long GetOrCreateClientDraftOrderID() {

            var idClient = 100; // TODO я
            var idCur = 0; //TODO грн
            var confOrders = EntityConfigDictionary.GetConfig("client_order");
            var ordersProv = new EntityProvider<ClientOrder_DTO>(confOrders);

            ClientOrder_DTO order = ordersProv.GetItems("id_client = :idclient", new OracleParameter("idclient", idClient))
                                                .FirstOrDefault();
            if (!(order == null))
            {
                return (long)order.id;
            }
            else
            {
                ClientOrder_DTO newDraftOrder = new ClientOrder_DTO();
                newDraftOrder.idClient = idClient;
                newDraftOrder.orderDate = DateTime.Now;
                newDraftOrder.idCur = idCur;
                newDraftOrder.total = 0; 
                newDraftOrder.idPaymentMethod = 1; //наличньіе
                newDraftOrder.idPaymentStatus = 1; //не оплачен
                newDraftOrder.idStatus = 0; //draft

                return (long)ordersProv.InsertItem(newDraftOrder).id;

            };

        }

    }
}
