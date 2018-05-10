using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class StoreReviewVote_DTO
    {
        public long? id { get; set; }
        public long? id_review { get; set; }
        public long? id_client { get; set; }
        public int? vote { get; set; }
    }
}
