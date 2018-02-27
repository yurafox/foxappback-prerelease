using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class PersonInfo_DTO
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string middleName { get; set; }
        public string passportSeries { get; set; }
        public string passportNum { get; set; }
        public string issuedAuthority { get; set; }
        public string taxNumber { get; set; }
        public DateTime birthDate { get; set; }
    }
}
