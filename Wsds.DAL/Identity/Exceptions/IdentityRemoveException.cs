using System;
using System.Collections.Generic;
using System.Text;

namespace Wsds.DAL.Identity.Exceptions
{
    public class IdentityRemoveException:Exception
    {
        public IdentityRemoveException(string message)
            :base(message)
        {}
    }
}
