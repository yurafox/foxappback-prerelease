using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace Wsds.DAL.ORM
{
    public partial class FoxStoreDBContext : DbContext
    {
        public long GetSequence (string SequenceName)
        {
            //            OracleParameter param1 = new OracleParameter("a", OracleDbType.Varchar2, 255);
            OracleParameter param2 = new OracleParameter("b", OracleDbType.Int64);

            //            param1.Direction = ParameterDirection.Input;
            param2.Direction = ParameterDirection.Output;

            //            param1.Value = Sequence;
            Database.ExecuteSqlCommand("begin select " + SequenceName + ".nextval into :b from dual; end;",
                new[] { param2 });
            return (long)(OracleDecimal)param2.Value;
        }
    }


}