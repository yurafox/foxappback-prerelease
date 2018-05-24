using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Infrastructure
{
    public class SCNMethodResult<T> where T : class
    {
        public SCNMethodResult(long scn, T obj) {
            SCN = scn;
            Result = obj;
        }

        public long SCN { get; set; }
        public T Result { get; set; }
    }
}
