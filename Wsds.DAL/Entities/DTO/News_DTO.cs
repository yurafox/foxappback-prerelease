using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class News_DTO
    {
        public int id { get; set; }
        [FieldBinding(Field = "preview_img_url")]
        public string previewImgUrl { get; set; }
        public string caption { get; set; }
        public string description { get; set; }
        [FieldBinding(Field = "public_date")]
        public DateTime publicDate { get; set; }
        public int categoryId { get; set; }
    }
}
