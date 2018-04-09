using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ClientOrderDatesRange_DTO
    {
        public string key { get; set; }
        public string displayName { get; set; }
        public bool? isDefault { get; set; }
    }
}
