using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class StoreReview_DTO
    {
        public long id { get; set; }
        public long idStore { get; set; }
        public long? idClient { get; set; }
        public string user { get; set; }
        public DateTime? reviewDate { get; set; }
        public string reviewText { get; set; }
        public int? rating { get; set; }
        public string advantages { get; set; }
        public string disadvantages { get; set; }
        public long? upvotes { get; set; }
        public long? downvotes { get; set; }
        public long? idReview { get; set; }
    }
}
