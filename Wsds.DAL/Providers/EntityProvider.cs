using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json;
using System;
using Wsds.DAL.Infrastructure;
using System.Linq;


namespace Wsds.DAL.Providers
{
    public class EntityProvider<T> where T: class
    {
        EntityConfig _config;
        private string _stmtCommit = "begin commit; end;";
        public EntityProvider(EntityConfig config) {
            _config = config;
        }
         
        public EntityConfig Config { get; set; }

        private object Convert2OraType(object value) {
            if (value is bool)
            {
                return (bool)value ? 1 : 0;
            }
            return value;
        }

        public void DeleteItem(long id, bool doCommit = true) {
            long KeyVal = id;
            string stmt = "begin delete from " + _config.BaseTable + " where " + _config.KeyField + " = :keyval ; end;";

            using (var con = new OracleConnection(_config.ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            using (var cmdCommit = new OracleCommand(_stmtCommit, con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter(":keyval", KeyVal));
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (doCommit)
                    {
                        cmdCommit.ExecuteNonQuery();
                    }
                }
                finally
                {
                    con.Close();
                }
            };
        }

        private long GetNextSeq()
        {
            long res = 0;
            string stmt = "select " + _config.Sequence + ".nextval as value from dual";
            using (var con = new OracleConnection(_config.ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        res = Convert.ToInt64(dr["value"].ToString());
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

        private class FieldMap {
            public string fldName { get; set; }
            public string fldParam { get; set; }

            public FieldMap(string _fldName, string _fldParam) {
                fldName = _fldName;
                fldParam = _fldParam;
            }
        }

        public T InsertItem(T item, bool doCommit = true) {
            IDictionary<FieldMap, OracleParameter> _dict = new Dictionary<FieldMap, OracleParameter>();
            int i = 0;
            long KeyVal = GetNextSeq();
            foreach (var prop in item.GetType().GetProperties())
            {
                string pname = prop.Name;
                bool isTransient = false;
                object[] attrib = prop.GetCustomAttributes(typeof(FieldBindingAttribute), true);
                if (attrib.Length > 0)
                {
                    FieldBindingAttribute bindAttrib = (FieldBindingAttribute)attrib[0];
                    pname = bindAttrib.Field;
                    isTransient = bindAttrib.IsTransient;
                }

                if (!isTransient) {
                    var _pName = ":param" + i.ToString();
                    OracleParameter _param =
                        ((pname).ToLower() == (_config.KeyField).ToLower())
                                ? new OracleParameter(_pName, KeyVal)
                                : new OracleParameter(_pName, Convert2OraType(prop.GetValue(item)));
                    _dict.Add(new FieldMap(pname, _pName), _param);
                    i++;
                }
            }

            var flds = "( " + string.Join(", ", _dict.Keys.Select(x => x.fldName)) + " )";
            var vals = "( " + string.Join(", ", _dict.Keys.Select(x => x.fldParam)) + " )";

            string stmt = "begin insert into " + _config.BaseTable + " " + flds + " values " + vals + "; end;";

            using (var con = new OracleConnection(_config.ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            using (var cmdCommit = new OracleCommand(_stmtCommit, con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.AddRange(_dict.Values.ToList().ToArray());
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (doCommit)
                    {
                        cmdCommit.ExecuteNonQuery();
                    }
                }
                finally
                {
                    con.Close();
                }
            };
            return GetItem(KeyVal);
        }

        public T UpdateItem(T item, bool doCommit = true) {
            IDictionary<string, OracleParameter> _dict = new Dictionary<string, OracleParameter>();
            int i = 0;
            long KeyVal = Convert.ToInt64(item.GetType().GetProperty(_config.KeyField).GetValue(item).ToString());
            foreach (var prop in item.GetType().GetProperties()) {
                string pname = prop.Name;
                bool isTransient = false;
                object[] attrib = prop.GetCustomAttributes(typeof(FieldBindingAttribute), true);
                if (attrib.Length > 0)
                {
                    FieldBindingAttribute bindAttrib = (FieldBindingAttribute)attrib[0];
                    pname = bindAttrib.Field;
                    isTransient = bindAttrib.IsTransient;
                }

                if (!isTransient && (!((pname).ToLower() == (_config.KeyField).ToLower()))) {
                    var _pName = ":param" + i.ToString();
                    _dict.Add(pname + " = " + _pName, new OracleParameter(_pName, Convert2OraType(prop.GetValue(item))));
                    i++;
                }
            }
            var setStmt = string.Join(", ", _dict.Keys);
            
            string stmt = "begin update " + _config.BaseTable + " set " + setStmt + " where " + _config.KeyField + " = :keyval ; end;";
            
            using (var con = new OracleConnection(_config.ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            using (var cmdCommit = new OracleCommand(_stmtCommit, con))

            {
                try
                {
                    con.Open();
                    cmd.Parameters.AddRange(_dict.Values.ToList().ToArray());
                    cmd.Parameters.Add(new OracleParameter(":keyval", KeyVal));
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (doCommit)
                    {
                        cmdCommit.ExecuteNonQuery();
                    }
                }
                finally
                {
                    con.Close();
                }
            };
            return GetItem(KeyVal);
        }

        public T GetItem(long id) {
            T res = null;
            string stmt;
            string valueFld = "value";
            if (_config.SerializerFunc != null)
            {
                stmt = "select " + _config.SerializerFunc + "(:a) as value from dual";

            }
            else
            {
                stmt = _config.SqlCommandSelect + " " + _config.SqlCommandWhere;
                stmt = stmt + (String.IsNullOrEmpty(_config.SqlCommandWhere) ? " where " : " and ")
                    + _config.KeyField + " = :a";
                valueFld = _config.ValueField;

            }
            using (var con = new OracleConnection(_config.ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    con.Open();
                    cmd.Parameters.Add(new OracleParameter("a", id));
                    OracleDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        string json = dr[valueFld].ToString();
                        if (!String.IsNullOrEmpty(json))
                        {
                            res = JsonConvert.DeserializeObject<T>(json);
                        }
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

        public IEnumerable<T> GetItems(string filterExpr = null, params OracleParameter[] args) {
            List<T> res = new List<T>();
            string stmt = _config.SqlCommandSelect + " " + _config.SqlCommandWhere;

            if (!(String.IsNullOrEmpty(filterExpr)))
                stmt = stmt + (String.IsNullOrEmpty(_config.SqlCommandWhere) ? " where " : " and ") 
                            + filterExpr + " " + _config.SqlCommandOrderBy;

            using (var con = new OracleConnection(_config.ConnString))
            using (var cmd = new OracleCommand(stmt, con))
            {
                try
                {
                    if (!(args == null))
                        cmd.Parameters.AddRange(args);
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    dr.FetchSize = cmd.RowSize * 100;
                    while (dr.Read())
                    {
                        var json = dr[_config.ValueField].ToString();
                        res.Add(
                                    JsonConvert.DeserializeObject<T>(json)
                                );
                    };
                }
                finally
                {
                    con.Close();
                }
            };
            return res;
        }

    }
}
