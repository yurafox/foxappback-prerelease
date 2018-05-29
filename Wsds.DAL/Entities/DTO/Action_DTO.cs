using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class Action_DTO
    {
        public long? id { get; set; }
        public string name { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public string img_url { get; set; }
        public long? priority { get; set; }
        public int? isActive { get; set; }
        public string sketch_content { get; set; }
        public string action_content { get; set; }
        public long? id_type { get; set; }
        public string badge_url { get; set;}
        public long? id_supplier { get; set; }
        public string title { get; set; }
        public byte? is_landing { get; set; }
    }
}
