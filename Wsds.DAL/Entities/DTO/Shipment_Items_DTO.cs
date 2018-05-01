using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Shipment_Items_DTO
    {
        public long id { get; set; }
        [FieldBinding(Field = "id_shipment")]
        public long idShipment { get; set; }
        [FieldBinding(Field = "id_order_spec_prod")]
        public long idOrderSpecProd { get; set; }
        public decimal? qty { get; set; }
        [FieldBinding(Field = "error_message")]
        public string errorMessage { get; set; }
    }
}
