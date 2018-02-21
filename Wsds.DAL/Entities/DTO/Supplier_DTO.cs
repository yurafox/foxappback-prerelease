using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Supplier_DTO : IDTO
    {
        public long id { get; set; }
        public string name { get; set; }
        public int? paymentMethodId { get; set; }
        public decimal? rating { get; set; }
        public decimal? positiveFeedbackPct { get; set; }
        public int? refsCount {get; set;}
    }
}
