using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class StoreReview_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "id_store")]
        public long? idStore { get; set; }
        [FieldBinding(Field = "id_client")]
        public long? idClient { get; set; }
        [FieldBinding(Field = "user_")]
        public string user { get; set; }
        [FieldBinding(Field = "review_date")]
        public DateTime? reviewDate { get; set; }
        [FieldBinding(Field = "review_text")]
        public string reviewText { get; set; }
        public int? rating { get; set; }
        public string advantages { get; set; }
        public string disadvantages { get; set; }
        public long? upvotes { get; set; }
        public long? downvotes { get; set; }
        [FieldBinding(Field = "parent_id")]
        public long? idReview { get; set; }
    }
}
