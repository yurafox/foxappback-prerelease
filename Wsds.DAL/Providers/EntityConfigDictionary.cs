using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wsds.DAL.Providers
{
    public static class EntityConfigDictionary
    {

        static Dictionary<string, EntityConfig> _configs;

        static EntityConfigDictionary() {
            _configs = new Dictionary<string, EntityConfig>();
        }

        public static void AddConfig(string key, EntityConfig config) {
            _configs.Add(key, config);
        }

        public static EntityConfig GetConfig(string config) {
            _configs.TryGetValue(config, out EntityConfig res);
            return res;
        }
    }
}
