using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wsds.DAL.Entities
{
    [Table("FOXSTORE.PRODUCT_PROP_VALUES")]
    public  class Product_Prop_Value
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long ID_PRODUCT { get; set; }

        public long ID_PROP { get; set; }

        [StringLength(255)]
        public string PROP_VALUE_STR { get; set; }

        public decimal? PROP_VALUE_NUMBER { get; set; }

        public bool? PROP_VALUE_BOOL { get; set; }

        public long? PROP_VALUE_ENUM { get; set; }

        public string PROP_VALUE_LONG { get; set; }

        public virtual Prop PROP { get; set; }

        public virtual Product PRODUCT { get; set; }

        public virtual Prop_Enums_List PropEnumsList { get; set; }
    }
}
