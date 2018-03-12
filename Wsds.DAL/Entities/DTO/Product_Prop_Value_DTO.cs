using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Product_Prop_Value_DTO
    {
        public long id { get; set; }
        public long id_Product { get; set; }
        public Prop_DTO id_Prop { get; set; }
        public string  prop_Value_Str { get; set; }
        public decimal? prop_Value_Number { get; set; }
        public bool? prop_Value_Bool { get; set; }
        //public long? prop_Value_Enum { get; set; }
        public Prop_Enum_List_DTO prop_Value_Enum { get; set; }
        public string prop_Value_Long { get; set; }
        public long? id_Measure_Unit { get; set; }
        public long? idx { get; set; }
        public long? out_bmask { get; set; }
    }
}
