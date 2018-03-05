using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Infrastructure;

namespace Wsds.DAL.Entities
{
    [Serializable]
    public class PersonInfo_DTO
    {
        public long? id { get; set; }
        [FieldBinding(Field = "FIRST_NAME")]
        public string firstName { get; set; }
        [FieldBinding(Field = "LAST_NAME")]
        public string lastName { get; set; }
        [FieldBinding(Field = "MIDDLE_NAME")]
        public string middleName { get; set; }
        [FieldBinding(Field = "PASSPORT_SERIES")]
        public string passportSeries { get; set; }
        [FieldBinding(Field = "PASSPORT_NUM")]
        public string passportNum { get; set; }
        [FieldBinding(Field = "ISSUED_AUTHORITY")]
        public string issuedAuthority { get; set; }
        [FieldBinding(Field = "TAX_NUMBER")]
        public string taxNumber { get; set; }
        [FieldBinding(Field = "BIRTH_DATE")]
        public DateTime birthDate { get; set; }
    }
}
