using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class ProductSupplCreditGrade_DTO
    {
        public long? id { get; set; }
        public long? idProduct { get; set; }
        public long? idSupplier { get; set; }
        public int? partsPmtCnt { get; set; }
        public int? creditSize { get; set; }
    }
}
