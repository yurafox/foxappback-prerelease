using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class BannerSlide_DTO
    {
        public long? id { get; set; }
        public string name { get; set; }
        [FieldBinding(Field = "img_url")]
        public string imgUrl { get; set; }
        [FieldBinding(Field = "action_type")]
        public int? actionType { get; set; }
        [FieldBinding(Field = "param_num")]
        public long? paramNum { get; set; }
        [FieldBinding(Field = "param_string")]
        public string paramString { get; set; }
    }
}
