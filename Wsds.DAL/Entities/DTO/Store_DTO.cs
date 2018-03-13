using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class Store_DTO
    {
        public long? id { get; set; }
        public long idCity { get; set; }
        public string address { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string openTime { get; set; }
        public string closeTime { get; set; }
        public long? rating { get; set; }
        public long? idFeedbacks { get; set; }
    }
}
