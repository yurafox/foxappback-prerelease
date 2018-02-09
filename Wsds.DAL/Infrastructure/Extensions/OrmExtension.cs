using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace Wsds.DAL.Infrastructure.Extensions
{
    public static class OrmExtension
    {
        public static List<T> IncludeLink<T>(this DbSet<T> store,Expression<Func<T,object>>[] includies, Func<T, bool> filter = null)
            where T:class
        {

            var currentEntity = store.AsNoTracking();
            var aggregateQuery = includies
                                 .Aggregate(currentEntity,(entity,include)=> (DbQuery<T>) entity.Include(include));


            return (filter==null) ? aggregateQuery.ToList() : aggregateQuery.Where(filter).ToList();
        }


        public static List<T> IncludeLink<T>(this DbSet<T> store, Func<T, bool> filter = null)
            where T : class
        {
            var currentEntity = store.AsNoTracking();
            return (filter == null) ? currentEntity.ToList() : currentEntity.Where(filter).ToList();
        }

        public static object WithOutEntityNavigation<T>(T data)
        {
            var props = data.GetType()
                            .GetProperties()
                            .Where(pr => pr.GetGetMethod().IsVirtual == false);

            dynamic temp = new ExpandoObject();
            foreach (var prop in props)
            {

                ((IDictionary<string, object>)temp).Add(prop.Name, prop.GetValue(data));
            }

            return temp;
        }
    }
}
