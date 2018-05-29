using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{
    [Serializable]
    public class NotifyOnProductArrivalRequest
    {
        public string email { get; set; }
        public long productId { get; set; }

    }
}
