using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Wsds.DAL.Providers;
using System.Linq;
using Wsds.DAL.Entities;

namespace Wsds.WebApp.Infrastructure
{
    public class AppDependencyResolver
    {
        private static IEnumerable<ICacheService<IDTO>> _collection;
        public static void InitCollection(IEnumerable<ICacheService<IDTO>> collection)
        {
            if (_collection == null)
                _collection = collection;
        }

        public static IEnumerable<ICacheService<IDTO>> GetAllServices() => _collection;
        public static ICacheService<IDTO> GetEntityByName(string name)
        {
            if (name == null)
                return null;

           return  _collection.FirstOrDefault(entity=>entity.EntityName.ToLower() == name);
        }
    }
}
