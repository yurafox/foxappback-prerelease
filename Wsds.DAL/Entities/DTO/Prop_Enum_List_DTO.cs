using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class Prop_Enum_List_DTO : IDTO
    {
        public long id { get; set; }
        public Prop_DTO id_Prop { get; set; }
        public string name { get; set; }
        public int? list_Index { get; set; }
        public int? bit_Index { get; set; }
    }
}
