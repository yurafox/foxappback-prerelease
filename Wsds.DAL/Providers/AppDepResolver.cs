using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsds.DAL.Entities;

namespace Wsds.DAL.Providers
{
    public class AppDepResolver
    {

        private static IEnumerable<ICacheService<object>> _collection;
        public static void InitCollection(IEnumerable<ICacheService<object>> collection)
        {
            if (_collection == null)
                _collection = collection;
        }

        public static IEnumerable<ICacheService<object>> GetAllServices() => _collection;
        public static ICacheService<object> GetEntityByName(string name)
        {

            if (name == null)
                return null;

            return _collection.FirstOrDefault(entity => entity.EntityName.ToLower() == name);
        }
    }
}
