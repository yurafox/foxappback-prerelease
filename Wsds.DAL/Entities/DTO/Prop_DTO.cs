using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Prop_DTO
    {
        public long id { get; set; }
        public string name { get; set; }
        public long? prop_type { get; set; }
        public bool? is_Multi_Select { get; set; }
        public string url { get; set; }
    }
}
