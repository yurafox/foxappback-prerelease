using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class CreditProduct_DTO
    {
        public long? sId { get; set; }
        public string sName { get; set; }
        public long? sDefProdId { get; set; }
        public int? sPartPay { get; set; }
        public int? sGracePeriod { get; set; }
        public int? maxTerm { get; set; }
        public decimal? firstPay { get; set; }
        public decimal? monthCommissionPct { get; set; }
        public decimal? yearPct { get; set; }
        public decimal? kpcPct { get; set; }
        public decimal? minAmt { get; set; }
        public decimal? maxAmt { get; set; }
        public int? minTerm { get; set; }
    }
}
