using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class Localization_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "component_name")]
        public string componentName { get; set; }
        [FieldBinding(Field = "tag_name")]
        public string tagName { get; set; }
        [FieldBinding(Field = "id_lang")]
        public int lang { get; set; }
        [FieldBinding(Field = "locale_text")]
        public string text { get; set; }
        [FieldBinding(Field = "is_front_or_back")]
        public int frontOrBack { get; set; }
    }
}
