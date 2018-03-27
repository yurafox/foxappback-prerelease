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
PUT product
{  
   "mappings":{  
      "_doc":{  
         "properties":{  
            "id":{  
               "type":"long"
            },
            "name":{  
               "type":"text"
            },
            "price":{  
               "type":"float"
            },
            "oldPrice":{  
               "type":"float"
            },
            "bonuses":{  
               "type":"float"
            },
            "manufacturerId":{  
               "type":"long"
            },
            "imageUrl":{  
               "type":"text"
            },
            "rating":{  
               "type":"short"
            },
            "recall":{  
               "type":"short"
            },
            "supplOffers":{  
               "type":"short"
            },
            "barcode":{  
               "type":"text"
            },
            "popularity":{  
               "type":"long"
            },
            "description":{  
               "type":"text"
            },
            "groups":{  
               "type":"nested",
               "properties":{  
                  "id":{  
                     "type":"long"
                  },
                  "name":{  
                     "type":"text"
                  }
               }
            },
            "manufacturer":{  
               "type":"object",
               "properties":{  
                  "id":{  
                     "type":"long"
                  },
                  "name":{  
                     "type":"text"
                  }
               }
            },
            "Props":{  
               "type":"nested",
               "properties":{  
                  "id":{  
                     "type":"long"
                  },
                  "id_Product":{  
                     "type":"long"
                  },
                  "id_Prop":{  
                     "type":"object"
                  },
                  "prop_Value_Number":{  
                     "type":"float"
                  },
                  "prop_Value_Bool":{  
                     "type":"byte"
                  },
                  "prop_Value_Enum":{  
                     "type":"object"
                  },
                  "id_Measure_Unit":{  
                     "type":"long"
                  },
                  "idx":{  
                     "type":"integer"
                  },
                  "out_bmask":{  
                     "type":"integer"
                  }
               }
            }
         }
      }
   }
}
     */

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
                            var path = paramsDict["index"] + j.ToString() + ".json";
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

            foreach (var file in fList) {
                var cmdText = " -s -H \"Content-Type: application/json\" -XPOST  \"" 
                    + paramsDict["esconnstring"] + "/_bulk\" --data-binary @" + file + " -o output_" + file + ".log";
                System.Diagnostics.Process.Start(paramsDict["curl"], cmdText);
            };


        }
    }
}
