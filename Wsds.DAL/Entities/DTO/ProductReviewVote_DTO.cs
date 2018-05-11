using System;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ProductReviewVote_DTO
    {
        public long? id { get; set; }
        public long? id_review { get; set; }
        public long? id_client { get; set; }
        public int? vote { get; set; }
    }
}
