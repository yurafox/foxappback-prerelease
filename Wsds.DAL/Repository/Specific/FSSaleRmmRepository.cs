using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using Serilog;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Wsds.DAL.Entities;
using Wsds.DAL.Entities.Communication;
using Wsds.DAL.Repository.Abstract;

namespace Wsds.DAL.Repository.Specific
{
    public class FSSaleRmmRepository : ISaleRmmRepository
    {
        private readonly IConfiguration _config;
        private readonly ICartRepository _cartRepository;
        private readonly ILogger _logger;

        public FSSaleRmmRepository(IConfiguration config, ICartRepository cartRepository, ILogger logger) {
            _config = config;
            _cartRepository = cartRepository;
            _logger = logger;
        } 

        public void CreateSaleRmm(ClientOrderMQ order)
        {   
            if (order.id == null)
                throw new ArgumentException("Id in order cann't be null");
            if (order.Client == null)
                throw new ArgumentException("idClient cann't be null");
            if (order.orderDate == null)
                throw new ArgumentException("orderDate cann't be null");

            var orderNum = $"{order.idApp}-{order.id}-{order.orderDate:yy}";
            Console.WriteLine($"Order num: {orderNum}");            
            var connString = _config.GetConnectionString("RmmConnection");

            using (var con = new OracleConnection(connString))
            {   
                con.Open();
                var transaction = con.BeginTransaction(IsolationLevel.ReadCommitted);
                foreach (var shipment in order.shipments)
                {
                    using (var cmd = new OracleCommand(HeaderQueryStr(), con))
                    {
                        var bonusPayAmount = GetBonusPayAmount(order, shipment);
                        cmd.Parameters.Add(new OracleParameter("SALE_ID", OracleDbType.Int32,
                            ParameterDirection.Output));
                        cmd.Parameters.Add(new OracleParameter("NET_ID", 10)); // foxtrot
                        cmd.Parameters.Add(new OracleParameter("ORDER_DATE", order.orderDate.Value.Date));
                        cmd.Parameters.Add(new OracleParameter("ORDER_NUM", orderNum));
                        cmd.Parameters.Add(new OracleParameter("KONTR_ID", 1488259));
                        cmd.Parameters.Add(new OracleParameter("CUR_ID", 4));
                        cmd.Parameters.Add(new OracleParameter("NOTES", string.Empty));
                        cmd.Parameters.Add(new OracleParameter("ORDER_TYPE_ID", order.idApp));
                        cmd.Parameters.Add(new OracleParameter("LO_ENTITY", shipment.idLoEntity));
                        cmd.Parameters.Add(new OracleParameter("DELIVERY_COST", shipment.loDeliveryCost));
                        cmd.Parameters.Add(new OracleParameter("KIND_DELIVERY", GetKindDelivery(shipment)));
                        cmd.Parameters.Add(new OracleParameter("LO_ENTITY_OFFICE", shipment.idLoEntityOffice));
                        cmd.Parameters.Add(new OracleParameter("STORE_PLACE", shipment.idStorePlace));
                        cmd.Parameters.Add(new OracleParameter("IS_PAYED", order.idPaymentStatus == 2 ? 1 : 0));
                        cmd.Parameters.Add(new OracleParameter("PAYMENT_METHOD", order.PaymentMethod.extDefaultIdT22));
                        cmd.Parameters.Add(new OracleParameter("BONUS_PAY_AMOUNT", bonusPayAmount));
                        cmd.Parameters.Add(new OracleParameter("BONUS_PAY", bonusPayAmount > 0 ? 1 : 0));
                        cmd.Parameters.Add(new OracleParameter("RECIPIENT_FIO", order.clientAddress.recName));
                        cmd.Parameters.Add(new OracleParameter("RECIPIENT_PHONE", order.Client.phone));
                        cmd.Parameters.Add(new OracleParameter("CLIENT_EMAIL", order.Client.email));
                        cmd.Parameters.Add(new OracleParameter("CLIENT_CARD_NUM", order.Client.barcode));
                        cmd.Parameters.Add(new OracleParameter("DELIVERY_CITY", order.clientAddress.idCity));
                        cmd.Parameters.Add(new OracleParameter("DELIVERY_ADDRESS", $"{order.clientAddress.street} {order.clientAddress.bldApp}"));
                        cmd.Parameters.Add(new OracleParameter("CREDIT_PRODUCT", order.idCreditProduct));
                        cmd.Parameters.Add(new OracleParameter("CLIENT_INN", order.PersonalInfo?.taxNumber));
                        cmd.Parameters.Add(new OracleParameter("BIRTH_DATE", order.PersonalInfo?.birthDate));
                        cmd.Parameters.Add(new OracleParameter("PASSPORT", $"{order.PersonalInfo?.passportSeries}{order.PersonalInfo?.passportNum}"));
                        cmd.Parameters.Add(new OracleParameter("WHERE_ISSUED", order.PersonalInfo?.issuedAuthority));

                        cmd.ExecuteNonQuery();
                        var saleId = long.Parse(cmd.Parameters["SALE_ID"].Value.ToString());
                        CreateRmmSpec(order, shipment, con, saleId);

                        _logger.Information($"Created new sale RMM, id:{saleId}");
                    }
                }
                transaction.Commit();
                con.Close();


                order.idStatus = 20;
                _cartRepository.UpdateClientOrder(order);
            }
        }

        private static decimal? GetBonusPayAmount(ClientOrderMQ order, Shipment_DTO shipment)
        {
            return (from shItem in shipment.shipmentItems
                join sp in order.specProducts on shItem.idOrderSpecProd equals sp.id
                select sp.payBonusCnt).Sum();
        }

        private static int GetKindDelivery(Shipment_DTO shipment)
        {
            int kindDelivery;
            if (shipment.idLoEntityOffice != null)
            {
                kindDelivery = 1;
            }
            else if (shipment.idStorePlace != null)
            {
                kindDelivery = 15;
            }
            else
            {
                kindDelivery = 2;
            }
            return kindDelivery;
        }

        private void CreateRmmSpec(ClientOrderMQ order, Shipment_DTO shipment, OracleConnection con, long saleId)
        {
            var orderSpec = (from shItem in shipment.shipmentItems
                join sp in order.specProducts on shItem.idOrderSpecProd equals sp.id
                select new
                {
                    shItem.qty,                    
                    sp.quotationProduct,
                    price = sp.price - (sp.payPromoCodeDiscount ?? 0),
                    sp.idAction,
                    sp.actionList,
                    sp.earnedBonusCnt,
                    sp.payBonusCnt,
                    sp.payPromoCode
                });
            foreach (var sp in orderSpec)
            {
                if (sp.quotationProduct == null)
                    throw new ArgumentException("QuotationProduct cann't be null");                
                using (var cmd = new OracleCommand(SpecQueryStr(), con))
                {   
                    cmd.Parameters.Add(new OracleParameter("SALE_ID", saleId));
                    cmd.Parameters.Add(new OracleParameter("PRODUCT_ID", sp.quotationProduct.idProduct));
                    cmd.Parameters.Add(new OracleParameter("QTY", sp.qty));
                    cmd.Parameters.Add(new OracleParameter("PRICE", sp.price));
                    cmd.Parameters.Add(new OracleParameter("PRODUCT_PRICE", sp.quotationProduct.price));
                    cmd.Parameters.Add(new OracleParameter("ACTION_ID", sp.idAction));
                    cmd.Parameters.Add(new OracleParameter("ACTION_LIST", sp.actionList));
                    cmd.Parameters.Add(new OracleParameter("EARNED_BONUS_CNT", sp.earnedBonusCnt));
                    cmd.Parameters.Add(new OracleParameter("PAY_BONUS_CNT", sp.payBonusCnt));
                    cmd.Parameters.Add(new OracleParameter("PROMOCODE", sp.payPromoCode));
                    cmd.Parameters.Add(new OracleParameter("IS_COMMIT", OracleDbType.Int32)).Value = 0;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static string HeaderQueryStr()
        {
            var builder = new StringBuilder();
            return builder                
                .AppendLine("begin :SALE_ID := typhoon.sales_imp_util.ImportSales5OK( ")
                .AppendLine(" ANetId    => :NET_ID, ")
                .AppendLine(" ADate     => :ORDER_DATE,     -- дата")
                .AppendLine(" AExtNum   => :ORDER_NUM,      -- Внешний №")
                .AppendLine(" AKId      => :KONTR_ID,       -- Код контрагента")
                .AppendLine(" ACurId    => :CUR_ID,         -- Код валюты")
                .AppendLine(" ANotes    => :NOTES,          -- Комментарий покупателя")
                .AppendLine(" pS_ORDER_TYPE_ID => :ORDER_TYPE_ID, -- Способ заказа - Мобильное приложение")
                .AppendLine(" AShtId    => :LO_ENTITY,      -- Код перевозчика")
                .AppendLine(" ADeliveryCost     => :DELIVERY_COST, -- Стоимость доставки")
                .AppendLine(" AKindDeliveryId   => :KIND_DELIVERY,")
                .AppendLine(" AShipmentStockId  => :LO_ENTITY_OFFICE, -- склад перевозчика car_wh_id")
                .AppendLine(" AShopKId  => :STORE_PLACE,    -- контрагент для пикапа")
                .AppendLine(" AIsByPayCard => :IS_PAYED, -- оплачено карточкой, да / нет")
                .AppendLine(" AFormPaymentId => :PAYMENT_METHOD, -- форма оплаты")
                .AppendLine(" aBonPayAmm => :BONUS_PAY_AMOUNT, -- сумма бонусов")
                .AppendLine(" aBonPay => :BONUS_PAY,")
                .AppendLine(" AFio => :RECIPIENT_FIO, -- ФИО")
                .AppendLine(" APhoneNum => :RECIPIENT_PHONE, -- Номер телевона")
                .AppendLine(" AEMail => :CLIENT_EMAIL, -- e-mail")
                .AppendLine(" AClientCardNum => :CLIENT_CARD_NUM, -- номер карты УПЛ")
                .AppendLine(" ACityId => :DELIVERY_CITY, -- Код города")
                .AppendLine(" AAddress => :DELIVERY_ADDRESS, -- Адрес")
                .AppendLine(" AHouseNum => null, -- Номер дома")
                .AppendLine(" AFlatNum =>  null, -- Номер квартиры")
                .AppendLine(" AKredProdId => :CREDIT_PRODUCT, -- кред.продукт")
                .AppendLine(" vs_client_inn => :CLIENT_INN,")
                .AppendLine(" ds_client_date_of_birth => :BIRTH_DATE,")
                .AppendLine(" vs_client_passport_number => :PASSPORT,")
                .AppendLine(" vs_client_pass_where_issued => :WHERE_ISSUED,")
                .AppendLine(" ATdId => null, -- Код типа доставки,")
                .AppendLine(" AMoneyXYSum => null, -- Сумма оплаты МОНЕХУ")
                .AppendLine(" AIsCommit => 0")
                .AppendLine("); end;")
                .ToString();
        }


        private static string SpecQueryStr()
        {
            var builder = new StringBuilder();
            return builder
                .AppendLine("begin typhoon.sales_imp_util.ImportSales5OK_Good( \n")                
                .AppendLine(" ASaleId => :SALE_ID,")
                .AppendLine(" AGoodId => :PRODUCT_ID,")
                .AppendLine(" AQty => :QTY,")
                .AppendLine(" APriceCur => :PRICE,")
                .AppendLine(" APriceSite => :PRODUCT_PRICE,")
                .AppendLine(" AActionId => :ACTION_ID,")
                .AppendLine(" AActionListId => :ACTION_LIST,")
                .AppendLine(" aBonCalc => :EARNED_BONUS_CNT,")
                .AppendLine(" aBonPay => :PAY_BONUS_CNT,")
                .AppendLine(" APromoCode => :PROMOCODE,")
                .AppendLine(" AIsCommit => :IS_COMMIT")
                .AppendLine(");")
                .AppendLine("end;")
                .ToString();
        }
    }
}
