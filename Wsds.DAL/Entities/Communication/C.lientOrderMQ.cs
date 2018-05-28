using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{
    [Serializable]
    public class ClientOrderMQ: ClientOrder_DTO
    {
        public IEnumerable<ClientOrderProduct_DTO> specProducts { get; set; }
        public ClientAddress_DTO clientAddress { get; set; }
        public IEnumerable<Shipment_DTO> shipments { get; set; }
    }
}
