using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ClientOrderProductsByDate_DTO
    {
        public long orderId { get; set; }
        public DateTime orderDate { get; set; }
        public long orderSpecId { get; set; }
        public long? idProduct { get; set; }
        public string productName { get; set; }
        public string productImageUrl { get; set; }
        public string loTrackTicket { get; set; }
        public long? idQuotation { get; set; }
        //public Product_DTO product { get; set; }
    }
}
