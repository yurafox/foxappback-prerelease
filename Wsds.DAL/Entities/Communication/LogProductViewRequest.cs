using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities.Communication
{
    public class LogProductViewRequest
    {
        public long idProduct { get; set; }
        public string viewParams { get; set; }
    }
}
