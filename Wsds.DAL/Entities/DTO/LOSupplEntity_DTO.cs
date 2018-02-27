using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class LoSupplEntity_DTO
    {
        public long? id { get; set; }
        public long? idSupplier { get; set; }
        public long? idLoEntity { get; set; }
    }
}
