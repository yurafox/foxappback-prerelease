using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wsds.WebApp.Auth
{
    public class SettingsModel
    {
        public long CurrencyId { get; set; }
        public long LangId { get; set; }
        public long IdApp { get; set; }
        public long? SCN { get; set; }

    }
}
