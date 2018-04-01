using System;
using System.Collections.Generic;
using System.IO;
using Oracle.ManagedDataAccess.Client;

namespace ESIndexer
{
    //index=product esconnstring="http://localhost:9200" oraconnstring="DATA SOURCE=//oratest10g64.mc.gcf:1521/FOXTROTUA; USER ID = FOXSTORE; PASSWORD=fox,store;" curl="c:\Program Files\Curl\bin\curl.exe" batchsize=2000
    //curl -s -H "Content-Type: application/json"  -XPOST "http://localhost:9200/_bulk" --data-binary @bulk.json -o output.txt
    //"c:\Program Files\Curl\bin\curl.exe" 
 
    /*
    Звпрос параметра cmdtext должен возвращать поля id и json_data 
    */
    class Program
    {
        private static Dictionary<string, string> ResolveArguments(string[] args)
        {
            if (args == null)
                return null;

            if (args.Length > 1)
            {
                var arguments = new Dictionary<string, string>();
                foreach (string argument in args)
                {
                    int idx = argument.IndexOf('=');
                    if (idx > 0)
                        arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                }
                return arguments;
            }

            return null;
        }

        static void Main(string[] args)
        {
            var paramsDict = ResolveArguments(args);
            var fList = new List<string>();

            StreamWriter tw = null;

            using (var con = new OracleConnection(paramsDict["oraconnstring"]))
            using (var cmd = new OracleCommand(paramsDict["cmdtext"], con))
            {
                try
                {
                    
                    con.Open();
                    OracleDataReader dr = cmd.ExecuteReader();
                    int i = 0; int j = 1;
                    while (dr.Read())
                    {
                        if (i % int.Parse(paramsDict["batchsize"]) == 0) {
                            if (tw != null) {
                                tw.Close();
                            };
                            string path = paramsDict["index"] + j.ToString() + ".json";
                            File.Create(path).Dispose();
                            tw = new StreamWriter(path, true);
                            fList.Add(path);
                            j++;
                        }
                        tw.WriteLine("{ \"index\" : { \"_index\" : \"" + paramsDict["index"] + 
                                     "\", \"_type\" : \"_doc\", \"_id\" : " + dr["id"].ToString() + " } }");
                        tw.WriteLine(dr["json_data"].ToString());
                        i++;
                    };
                }
                finally
                {
                    con.Close();
                    tw.Close();
                }
            };

            foreach (string file in fList) {
                string cmdParams = " -s -H \"Content-Type: application/json\" -XPOST  \"" 
                    + paramsDict["esconnstring"] + "/_bulk\" --data-binary @" + file + " -o output_" + file + ".log";
                System.Diagnostics.Process.Start(paramsDict["curl"], cmdParams);
            };


        }
    }
}
