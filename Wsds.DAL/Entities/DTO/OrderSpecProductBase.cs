using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    public class OrderSpecProductBase
    {
        public long? id { get; set; }
        [FieldBinding(Field = "id_order")]
        public long? idOrder { get; set; }
        [FieldBinding(Field = "id_quotation")]
        public long? idQuotationProduct { get; set; }
        public decimal? price { get; set; }
        public decimal? qty { get; set; }
        [FieldBinding(Field = "id_store_place")]
        public long? idStorePlace { get; set; }
        [FieldBinding(Field = "earned_bonus_cnt")]
        public decimal? earnedBonusCnt { get; set; }
    }
}
