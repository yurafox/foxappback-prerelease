using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    public abstract class DTOAbstract
    {
        long id { get; set; }
        protected string SqlCommandSelect { get; set; }
        protected string SqlCommandWhere { get; set; }
        protected string KeyField { get; set; }
        protected string ValueField { get; set; }
        protected string PreserializedJSONField { get; set; }
        protected string SerializerFunc { get; set; }

    }
}
