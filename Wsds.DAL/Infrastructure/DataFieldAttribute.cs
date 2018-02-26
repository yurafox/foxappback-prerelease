using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Infrastructure
{
    public class FieldBindingAttribute : Attribute
    {
        public string Field { get; set; }
    }
}
