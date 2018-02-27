using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class LoTrackLog
    {
        public long? id { get; set; }
        public long? idOrderSpecProd { get; set; }
        public DateTime? trackDate { get; set; }
        public string trackString { get; set; }
    }
}
