using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class Novelty_DTO
    {
        public long? id { get; set; }
        public long? idProduct { get; set; }
        public string name { get; set; }
        public string img_url { get; set; }
        public long? priority { get; set; }
        public string sketch_content { get; set; }
        public string novelty_content { get; set; }
    }
}
