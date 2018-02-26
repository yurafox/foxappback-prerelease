using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class LoEntity_DTO
    {
        public long? id { get; set; }
        public string name { get; set; }
    }
}
