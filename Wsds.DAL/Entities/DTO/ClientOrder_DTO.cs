using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ClientOrder_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "order_date")]
        public DateTime? orderDate { get; set; }
        [FieldBinding(Field = "id_cur")]
        public long? idCur { get; set; }
        [FieldBinding(Field = "id_client")]
        public long? idClient { get; set; }
        public decimal? total { get; set; }
        [FieldBinding(Field = "id_payment_method")]
        public long? idPaymentMethod { get; set; }
        [FieldBinding(Field = "id_payment_status")]
        public long? idPaymentStatus { get; set; }
        [FieldBinding(Field = "id_status")]
        public long? idStatus { get; set; }
        [FieldBinding(Field = "lo_id_client_address")]
        public long? loIdClientAddress { get; set; }
        [FieldBinding(Field = "items_total")]
        public decimal? itemsTotal { get; set; }
        [FieldBinding(Field = "shipping_total")]
        public decimal? shippingTotal { get; set; }
        [FieldBinding(Field = "bonus_total")]
        public decimal? bonusTotal { get; set; }
        [FieldBinding(Field = "promobonus_total")]
        public decimal? promoBonusTotal { get; set; }
        [FieldBinding(Field = "bonus_earned")]
        public decimal? bonusEarned { get; set; }
        [FieldBinding(Field = "promocodedisc_total")]
        public decimal? promoCodeDiscTotal { get; set; }
        [FieldBinding(Field = "id_person")]
        public long? idPerson { get; set; }
        [FieldBinding(IsTransient = true)]
        public string clientHistFIO { get; set; }
        [FieldBinding(IsTransient = true)]
        public string clientHistAddress { get; set; }
        [FieldBinding(Field = "id_credit_product")]
        public long? idCreditProduct { get; set; }
        [FieldBinding(Field = "credit_period")]
        public long? creditPeriod { get; set; }
        [FieldBinding(Field = "credit_monthly_pmt")]
        public decimal? creditMonthlyPmt { get; set; }
        [FieldBinding(Field = "id_app")]
        public long? idApp { get; set; }
        public long? scn { get; set; }

    }
}
