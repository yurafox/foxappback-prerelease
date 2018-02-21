using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Manufacturer_DTO : IDTO
    {
        public long id { get; set; }
        public string name { get; set; }
    }
}
