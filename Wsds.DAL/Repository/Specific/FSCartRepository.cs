using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Infrastructure;
using Wsds.DAL.Providers;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSCartRepository : ICartRepository
    {
        private readonly IClientRepository _clRepo;
        private readonly IConfiguration _config;
        private ICacheService<Quotation_Product_DTO> _csQProduct;
        private IProductRepository _prodRepo;
        private IQuotationProductRepository _qpRepo; 

        public FSCartRepository(IClientRepository clRepo, 
                                IConfiguration config,
                                IProductRepository prodRepo,
                                IQuotationProductRepository qpRepo,
                                ICacheService<Quotation_Product_DTO> csQProduct)
        {
            _clRepo = clRepo;
            _config = config;
            _prodRepo = prodRepo;
            _qpRepo = qpRepo;
            _csQProduct = csQProduct;
        }

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


        private bool CanUpdateOrder(long id,long idClient) {
            var cOrders = EntityConfigDictionary.GetConfig("client_order");
            var ordersProv = new EntityProvider<ClientOrder_DTO>(cOrders);

            ClientOrder_DTO order = ordersProv.GetItems("id_client = :idclient and id = :idOrder",
                                                        new OracleParameter("idclient", idClient),
                                                        new OracleParameter("idOrder", id)
                                                        )
                                                .FirstOrDefault();

            return ((order != null) && (order.idStatus == 0)); 
        }

        private void UpdateShipmentItem(long id, decimal qty)
        {
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin update shipment_items a set qty = :a where a.id_order_spec_prod = :b ; commit; end;", con))
            {
                try
                {
                    cmd.Parameters.Add(new OracleParameter("a", qty));
                    cmd.Parameters.Add(new OracleParameter("b", id));
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            };
        }

        public SCNMethodResult<ClientOrderProduct_DTO> UpdateCartProduct(ClientOrderProduct_DTO item,long clientId)
        {
            if (CanUpdateOrder((long)item.idOrder, clientId))
            {
                var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
                var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);
                var it = prov.UpdateItem(item);
                UpdateShipmentItem((long)it.id, (decimal)it.qty);

                // in case of complect update second item qty = qty of main item in complect
                var cmpl = it.complect;
                if (cmpl != null) {
                    var complectItems = prov.GetItems("t.complect = :complect and t.id_order = :idorder and t.id <> :id",
                                    new OracleParameter("complect", cmpl),
                                    new OracleParameter("idOrder", item.idOrder),
                                    new OracleParameter("id", item.id)
                                    );

                    foreach (var ci in complectItems) {
                        ci.qty = it.qty;
                        prov.UpdateItem(ci);
                        UpdateShipmentItem((long)ci.id, (decimal)ci.qty);
                    }
                }

                return new SCNMethodResult<ClientOrderProduct_DTO> (223, it);
            }
            else return null; //if order does not exists => do nothing and return null
        }

        public ClientOrderProduct_DTO InsertCartProduct(ClientOrderProduct_DTO item, long clientId, long currency, long idApp)
        {

            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);
            long _idOrder = (long)GetOrCreateClientDraftOrder(clientId, currency, idApp).id;

            var fndSpec = prov.GetItems("t.id_quotation = :qp and t.id_order = :id_order and t.complect = :complect",
                                        new OracleParameter("qp", item.idQuotationProduct),
                                        new OracleParameter("id_Order", _idOrder),
                                        new OracleParameter("complect", item.complect))
                                       .FirstOrDefault();
            if (fndSpec != null)
            {
                fndSpec.qty = fndSpec.qty + item.qty;
                return prov.UpdateItem(fndSpec);
            }
            else
            {
                item.idOrder = _idOrder;
                return prov.InsertItem(item);
            }
        }

        private void DeleteShipmentItem(long id) {
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin delete from shipment_items a where a.id_order_spec_prod = :b ; commit; end;", con))
            {
                try
                {
                    cmd.Parameters.Add(new OracleParameter("b", id));
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
                finally
                {
                    con.Close();
                }
            };
        }

        public void DeleteCartProduct(long id, long clientId)
        {
            
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);
            var item = prov.GetItem(id);

            if (!CanUpdateOrder((long)item.idOrder, clientId))
                return;




            // Удяляем составньіе комплекта
            var cmpl = item.complect;
            if (cmpl != null)
            {
                var secondItems = prov.GetItems("t.complect = :complect and t.id_order = :idorder and t.id <> :id",
                                new OracleParameter("complect", cmpl),
                                new OracleParameter("idOrder", item.idOrder),
                                new OracleParameter("id", item.id)
                                );
                foreach (var sitem in secondItems) {
                    DeleteShipmentItem((long)sitem.id);
                    prov.DeleteItem((long)sitem.id);
                }
            }

            DeleteShipmentItem(id);
            prov.DeleteItem(id);

        }

        public ClientOrder_DTO GetOrCreateClientDraftOrder(long clientId, long currencyId, long idApp) {
            var confOrders = EntityConfigDictionary.GetConfig("client_order");
            var ordersProv = new EntityProvider<ClientOrder_DTO>(confOrders);

            ClientOrder_DTO order = ordersProv.GetItems("id_client = :idclient", new OracleParameter("idclient", clientId))
                                                .FirstOrDefault();
            if (!(order == null))
            {
                return order;
            }
            else
            {
                ClientOrder_DTO newDraftOrder = new ClientOrder_DTO();
                newDraftOrder.idClient = clientId;
                newDraftOrder.orderDate = DateTime.Now;
                newDraftOrder.idCur = currencyId;
                newDraftOrder.total = 0; 
                newDraftOrder.idPaymentMethod = 1; //наличньіе
                newDraftOrder.idPaymentStatus = 1; //не оплачен
                newDraftOrder.idStatus = 0; //draft
                newDraftOrder.idApp = idApp;
                newDraftOrder.scn = GenerateSCN();

                return ordersProv.InsertItem(newDraftOrder);
            };
        }

        public IEnumerable<ClientOrderProduct_DTO> GetClientOrderProductsByOrderId(long orderId) //TODO обьєдинить с данньіми из Т22
        {
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product_all");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);

            return prov.GetItems("t.id_order = :orderId", new OracleParameter("orderId", orderId));
        }

        public IEnumerable<ClientOrder_DTO> GetClientOrders(long clientId) //TODO обьєдинить с данньіми из Т22
        {
            var coaCnfg = EntityConfigDictionary.GetConfig("client_order_all");
            var prov = new EntityProvider<ClientOrder_DTO>(coaCnfg);

            return prov.GetItems("t.id_client = :idClient", new OracleParameter("idClient", clientId))
                    .OrderByDescending(x => x.orderDate);
        }

        public ClientOrder_DTO SaveClientOrder(ClientOrder_DTO order, long clientId)
        {
            var confOrders = EntityConfigDictionary.GetConfig("client_order");
            var ordersProv = new EntityProvider<ClientOrder_DTO>(confOrders);
            if (CanUpdateOrder((long)order.id,clientId))
            {
                return ordersProv.UpdateItem(order);
            }
            else
                return null;
        }

        /*
        private ClientOrderProduct_DTO MapItem(long g_id, long? action_id, long? is_set, CalculateCartRequest cartObj) {

            return cartObj.cartContent.FirstOrDefault(
                            x => (_csQProduct.Item((long)x.idQuotationProduct).idProduct == g_id) 
                               );
        }
        */

        public IEnumerable<CalculateCartResponse> CalculateCart(CalculateCartRequest cartObj, long card, long clientId, long currency, long idApp)
        {

            var ordId = GetOrCreateClientDraftOrder(clientId, currency,idApp).id;
            var itemsList = new List<CalcCartRequestT22_Item>();

            var confOrdersSpec = EntityConfigDictionary.GetConfig("client_order_product");
            var ordersSpecProv = new EntityProvider<ClientOrderProduct_DTO>(confOrdersSpec);
            var orderSpec = ordersSpecProv.GetItems("t.id_order = :idOrder", new OracleParameter("idOrder", ordId));

            foreach (var orderLine in orderSpec)
            {
                var s = new CalcCartRequestT22_Item
                {
                    pk_id = orderLine.id,
                    g_id = _csQProduct.Item((long)orderLine.idQuotationProduct).idProduct,
                    qty = orderLine.qty,
                    price = orderLine.price,
                    is_set = (orderLine.complect == null) ? 0 : 1,
                    act = orderLine.idAction
                };
                itemsList.Add(s);
            }

            string barcode = $"+{card}"; //"+11049778713";

            var calcCartRequestT22 = new CalcCartRequestT22
            {
                cnum = barcode, 
                lptype = 2, //TODO ?
                promocode = cartObj.promoCode,
                bonpayamm = cartObj.maxBonusCnt,
                dogrole = 12, //T22 constant 
                is_cred_prod = cartObj.creditProductId,
                bait_bon = ((bool)cartObj.usePromoBonus) ? 1 : 0,
                spec = itemsList
            };

            var requestJson = JsonConvert.SerializeObject(calcCartRequestT22);
            //Debug.WriteLine(requestJson);

            string res = "";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand("begin :result := foxstore.pkg_xml_json.get_SaleSpec_Js(:json, :key); end;", con))
            {
                try
                {
                    cmd.Parameters.Add("result", OracleDbType.Varchar2, ParameterDirection.Output);
                    cmd.Parameters["result"].Size = 32767;
                    cmd.Parameters.Add(new OracleParameter("json", requestJson));
                    cmd.Parameters.Add(new OracleParameter("key", barcode));
                    con.Open();
                    cmd.ExecuteNonQuery();
                    res = cmd.Parameters["result"].Value.ToString();
                }
                finally
                {
                    con.Close();
                }
            };
            //Debug.WriteLine(res);
            var resp = JsonConvert.DeserializeObject<CalcCartResponseT22>(res);

            IList<CalculateCartResponse> lst = new List<CalculateCartResponse>();

            if (resp != null) { 
                foreach (var item in resp.outspec) {
                    var _promoCodeDisc = 0; // (item.promocode == "") ? 0 : Decimal.Parse(item.promocode, new CultureInfo("ru-RU"));
                    var _bonusDisc = item.bonus_pay;
                    var _promoBonusDisc = item.bonus_sp_pay;
                    var _earnedBonus =item.bonus;
                    var _pk = item.pk_id;
                    var _qty = item.qty;

 
                    lst.Add(
                                new CalculateCartResponse {
                                    clOrderSpecProdId = _pk,
                                    promoCodeDisc = _promoCodeDisc,
                                    bonusDisc= _bonusDisc,
                                    promoBonusDisc = _promoBonusDisc,
                                    earnedBonus = _earnedBonus,
                                    qty = _qty
                                }
                              );
                }
            }
            return lst;
        }


        private decimal GetShiippingTotal(long idOrder) {
            var ship = EntityConfigDictionary.GetConfig("shipment");
            var shipProv = new EntityProvider<Shipment_DTO>(ship);
            return (decimal)shipProv.GetItems("id_order = :idOrder", new OracleParameter("idOrder", idOrder))
                .Sum(s => s.loDeliveryCost);
        }

        public PostOrderResponse PostOrder(ClientOrder_DTO order)
        {

            long clientId = order.idClient.Value;
            var res = new PostOrderResponse { isSuccess = false, errorMessage = "Your cart is empty" };

            var confOrders = EntityConfigDictionary.GetConfig("client_order");
            var ordersProv = new EntityProvider<ClientOrder_DTO>(confOrders);
            var dbOrder = ordersProv.GetItems("t.id_client = :idClient and t.id = :idOrder and t.id_status=0", 
                                                new OracleParameter("idClient", clientId),
                                                new OracleParameter("idOrder", order.id)
                                              )
                                  .FirstOrDefault();

            if (dbOrder != null) { 
                var confOrdersSpec = EntityConfigDictionary.GetConfig("client_order_product");
                var ordersSpecProv = new EntityProvider<ClientOrderProduct_DTO>(confOrdersSpec);
                var orderSpecCnt = ordersSpecProv.GetItems("t.id_order = :idOrder", new OracleParameter("idOrder", dbOrder.id))
                                                 .Count();

                if (orderSpecCnt > 0)
                {
                    dbOrder.idStatus = 1;


                    var sums = 
                        ordersSpecProv.GetItems("t.id_order = :idOrder", new OracleParameter("idOrder", dbOrder.id))
                        .GroupBy(x => x.idOrder)
                        .Select(x => new {
                                            _itemsTotal = x.Sum(s => s.qty * s.price),
                                            /*_shippingTotal = x.Sum(s => s.loDeliveryCost),*/
                                            _promoBonusTotal = x.Sum(s => s.payPromoBonusCnt * s.qty),
                                            _bonusTotal = x.Sum(s => s.payBonusCnt * s.qty),
                                            _bonusEarned = x.Sum(s => s.earnedBonusCnt * s.qty)
                                         }
                               );
                    dbOrder.itemsTotal = sums.FirstOrDefault()._itemsTotal;
                    dbOrder.shippingTotal = GetShiippingTotal((long)dbOrder.id); //sums.FirstOrDefault()._shippingTotal;
                    dbOrder.promoBonusTotal = sums.FirstOrDefault()._promoBonusTotal;
                    dbOrder.bonusTotal = sums.FirstOrDefault()._bonusTotal;
                    dbOrder.bonusEarned = sums.FirstOrDefault()._bonusEarned;
                    dbOrder.total = dbOrder.itemsTotal + dbOrder.shippingTotal - dbOrder.promoBonusTotal - dbOrder.bonusTotal;

                    if (
                        (dbOrder.itemsTotal == order.itemsTotal)
                        &&
                        (dbOrder.shippingTotal == order.shippingTotal)
                        &&
                        (dbOrder.promoBonusTotal == order.promoBonusTotal)
                        &&
                        (dbOrder.bonusTotal == order.bonusTotal)
                       )
                    {
                        ordersProv.UpdateItem(dbOrder);
                        return new PostOrderResponse { isSuccess = true, errorMessage = "" };
                    }
                    else
                    {
                        return new PostOrderResponse { isSuccess = false, errorMessage = "Your order has been changed from another device" };
                    };

                }
            };
            return res;
        }

        public IEnumerable<ClientOrderProductsByDate_DTO> GetOrderProductsByDate(string datesRange, long clientId)
        {
            var ConnString = _config.GetConnectionString("MainDataConnection");
            DateTime d1 = new DateTime();
            DateTime d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 00, 00);

            if (datesRange.ToLower() == "30d")
            {
                d1 = d2.AddDays(-31);
            }
            else if (datesRange.ToLower() == "6m")
            {
                d1 = d2.AddDays(-183);
            }
            else {
                int year = Int32.Parse(datesRange);
                d1 = new DateTime(year, 01, 01);
                d2 = new DateTime(year, 12, 31);
            };
            List<ClientOrderProductsByDate_DTO> res = new List<ClientOrderProductsByDate_DTO>();
            using (var con = new OracleConnection(ConnString))
            using (var histCreationCmd = new OracleCommand("begin Pkg_Synch_Data.FillClientOrderHistory(pclientid => :clientId, pdatestart => :pdatestart, pdateend => :pdateend); end;", con))
            using (var cmd = new OracleCommand("select o.id as orderId, o.order_date, s.id as orderSpecId, s.id_product, s.id_quotation_product, s.lo_track_ticket " +
                                               "from CLIENT_ORDER_HISTORY o, ORDER_HISTORY_SPEC_PRODUCTS s " +
                                               "where o.id = s.id_order and o.id_client = :idClient " +
                                               "and trunc(o.order_date) between :d1 and :d2 order by o.order_date desc", con))
            /*using (var cmd = new OracleCommand("select o.id as orderId, o.order_date, s.id as orderSpecId, qp.id_product, s.lo_track_ticket, s.id_quotation " +
                                                "from CLIENT_ORDERS o, ORDER_SPEC_PRODUCTS s, QUOTATIONS_PRODUCTS qp " +
                                                "where o.id = s.id_order and o.id_client = :idClient and s.id_quotation = qp.id " +
                                                "and trunc(o.order_date) between :d1 and :d2 and o.id_status>0 and o.id_status<=200 " +
                                                "order by o.order_date desc", con))
            */
            {
                try
                {
                    con.Open();

                    histCreationCmd.Parameters.Add("clientId", clientId);
                    histCreationCmd.Parameters.Add("pdatestart", (OracleDate)d1);
                    histCreationCmd.Parameters.Add("pdateend", (OracleDate)d2);
                    histCreationCmd.ExecuteNonQuery();

                    cmd.Parameters.Add(new OracleParameter("idClient", clientId));
                    OracleParameter fromDate = new OracleParameter("d1", OracleDbType.Date)
                    {
                        Value = (OracleDate)d1
                    };
                    cmd.Parameters.Add(fromDate);

                    OracleParameter toDate = new OracleParameter("d2", OracleDbType.Date)
                    {
                        Value = (OracleDate)d2
                    };
                    cmd.Parameters.Add(toDate);

                    

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {

                        var qpId = (long)reader["id_quotation_product"];
                        //_qpRepo
                        Quotation_Product_DTO qp = _qpRepo.QuotationProduct(qpId);

                        var prodId = qp.idProduct; //(long)reader["id_product"];
                        Product_DTO product = _prodRepo.Product(prodId);

                        ClientOrderProductsByDate_DTO item = new ClientOrderProductsByDate_DTO
                        {
                            orderId = (long)reader["orderid"],
                            orderDate = (DateTime)reader["order_date"],
                            orderSpecId = (long)reader["orderspecid"],
                            idProduct = Convert.IsDBNull(reader["id_product"]) ? null : (long?)reader["id_product"],
                            productName = product.name,
                            productImageUrl = product.imageUrl,
                            loTrackTicket = reader["lo_track_ticket"].ToString(),
                            idQuotation = Convert.IsDBNull(reader["id_quotation_product"]) ? null : (long?)reader["id_quotation_product"]
                        };
                        res.Add(item);
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

        /*
        public ClientOrder_DTO GetClientOrder(long orderId, long idClient)
        {
            var coaCnfg = EntityConfigDictionary.GetConfig("client_order_all");
            var prov = new EntityProvider<ClientOrder_DTO>(coaCnfg);

            return prov.GetItems("t.id_client = :idClient and t.id = :id", 
                                 new OracleParameter("idClient", idClient),
                                 new OracleParameter("id", orderId)
                                 ).FirstOrDefault();
        }
        */

        public ClientOrder_DTO GetClientHistOrder(long orderId, long idClient)
        {
            var coaCnfg = EntityConfigDictionary.GetConfig("client_order_hist");
            var prov = new EntityProvider<ClientOrder_DTO>(coaCnfg);

            return prov.GetItems("t.id_client = :idClient and t.id = :id",
                                 new OracleParameter("idClient", idClient),
                                 new OracleParameter("id", orderId)
                                 ).FirstOrDefault();
        }

        public IEnumerable<ClientOrderProduct_DTO> GetClientHistOrderProductsByOrderId(long orderId)
        {
            var qpCnfg = EntityConfigDictionary.GetConfig("client_order_product_hist");
            var prov = new EntityProvider<ClientOrderProduct_DTO>(qpCnfg);
            return prov.GetItems("t.id_order = :orderId", new OracleParameter("orderId", orderId));
        }

        public IEnumerable<Shipment_DTO> GenerateShipments(long clientId, long currencyId, long idApp)
        {
            var idOrder = GetOrCreateClientDraftOrder(clientId, currencyId,idApp).id;
            var res = new List<Shipment_DTO>();
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmdGenShpmt = new OracleCommand("begin foxstore.lo.Generate_Shipment(:idOrder); end;", con))
            {
                try
                {
                    cmdGenShpmt.Parameters.Add(new OracleParameter("idOrder", idOrder));
                    con.Open();
                    cmdGenShpmt.ExecuteNonQuery();

                    var shCnfg = EntityConfigDictionary.GetConfig("shipment");
                    var prov = new EntityProvider<Shipment_DTO>(shCnfg);
                    res = prov.GetItems("t.id_order = :orderId", new OracleParameter("orderId", idOrder)).ToList();
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

        public Shipment_DTO SaveShipment(Shipment_DTO shipment, long idClient)
        {
            var shCnfg = EntityConfigDictionary.GetConfig("shipment");
            var prov = new EntityProvider<Shipment_DTO>(shCnfg);

            return prov.UpdateItem(shipment);
        }

        private long GenerateSCN()
        {
            long res = 0;
            string stmt = "select seq_scn.nextval as value from dual";
            var ConnString = _config.GetConnectionString("MainDataConnection");
            using (var con = new OracleConnection(ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        res = Convert.ToInt64(dr["value"].ToString());
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
