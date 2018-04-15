using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ActionsByProduct_DTO
    {
        public long? actionId { get; set; }
        public long? actionType { get; set; }
        public long? idQuotationProduct { get; set; }
        public long? idProduct { get; set; }
        public long? idCur { get; set; }
        public decimal? actionPrice { get; set; }
        public decimal? regularPrice { get; set; }
        public decimal? bonusQty { get; set; }
        public string productName { get; set; }
        public string complect { get; set; }
        public int? isMain { get; set; }
        public long? idGroup { get; set; }
        public string imgUrl { get; set; }
        public string title { get; set; }

    }
}
