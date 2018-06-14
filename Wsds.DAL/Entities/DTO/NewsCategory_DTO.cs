using System;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class NewsCategory_DTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int sort { get; set; }
    }
}
