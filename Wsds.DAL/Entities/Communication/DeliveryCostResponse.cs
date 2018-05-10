using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{

    /*
    [Serializable]
    public class DeliveryResponseT22_S_Cost
    {
        public long? g_id { get; set; }
        public decimal? deliv { get; set; }
        public decimal? deliv_floor { get; set; }
    }
    */

    [Serializable]
    public class DeliveryResponseT22_Cost
    {
        public decimal? deliv { get; set; }
        public decimal? deliv_floor { get; set; }
        //public IEnumerable<DeliveryResponseT22_S_Cost> spec { get; set; }
    }
}
