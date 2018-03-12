using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{
    [Serializable]
    public class DeliveryResponseT22_S_Date
    {
        public long? g_id { get; set; }
        public string deliv_date { get; set; }
    }

    //[Serializable]
    //public class DeliveryResponseT22_Spec_Date
    //{
    //    public IEnumerable<DeliveryResponseT22_S_Date> s { get; set; }
    //}

    [Serializable]
    public class DeliveryResponseT22_Date
    {
        public IEnumerable<DeliveryResponseT22_S_Date> spec { get; set; }
    }
}
