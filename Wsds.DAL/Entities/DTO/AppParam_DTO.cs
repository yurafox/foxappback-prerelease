﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class AppParam_DTO
    {
        public long id { get; set; }
        public string propName { get; set; }
        public string propVal { get; set; }
    }
}