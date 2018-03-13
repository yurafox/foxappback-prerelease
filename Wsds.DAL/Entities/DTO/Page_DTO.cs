using System;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class Page_DTO
    {
        public long? id { get; set; }
        public string name { get; set; }
        public string content { get; set; }
    }
}
