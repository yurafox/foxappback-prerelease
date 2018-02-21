using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using Newtonsoft.Json;
using System;

namespace Wsds.DAL.Providers
{
    public class EntityProvider<T> where T: class
    {
        EntityConfig _config;
        public EntityProvider(EntityConfig config) {
            _config = config;
        }

        public EntityConfig Config { get; set; }

        public T SetItem(T item) {
            //TODO implement insert method
            return item;
        }
        public T GetItem(long id) {
            T res = null;
            string stmt = "select " + _config.SerializerFunc + "(:a) as value from dual";
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
                        string json = dr["value"].ToString();
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
                stmt = stmt + (String.IsNullOrEmpty(_config.SqlCommandWhere) ? " " : " and ") + filterExpr;

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
