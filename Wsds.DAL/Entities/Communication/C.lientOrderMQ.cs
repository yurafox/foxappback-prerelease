using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;
using Wsds.DAL.Repository.Specific;

namespace Wsds.DAL.Entities.Communication
{
    [Serializable]
    public class ClientOrderMQ: ClientOrder_DTO
    {
        [FieldBinding(IsTransient = true)]
        public IEnumerable<ClientOrderProduct_DTO> specProducts { get; set; }
        [FieldBinding(IsTransient = true)]
        public ClientAddress_DTO clientAddress { get; set; }
        [FieldBinding(IsTransient = true)]
        public IEnumerable<Shipment_DTO> shipments { get; set; }
        [FieldBinding(IsTransient = true)]
        public Enum_Pmt_Method_DTO PaymentMethod { get; set; }
        [FieldBinding(IsTransient = true)]
        public Client_DTO Client { get; set; }
        [FieldBinding(IsTransient = true)]
        public PersonInfo_DTO PersonalInfo { get; set; }
    }
}
