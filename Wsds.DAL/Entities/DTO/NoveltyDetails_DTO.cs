using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class NoveltyDetails_DTO
    {
        public long? id { get; set; }
        public long idNovelty { get; set; }
        public long idProduct { get; set; }
    }
}
