using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.DTO
{
    [Serializable]
    public class Poll_DTO
    {
        public long? id { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
        public string urlBanner { get; set; }
        public string bannerText { get; set; }
    }
}
