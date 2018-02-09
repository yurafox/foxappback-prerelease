using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace Wsds.DAL.Infrastructure.Extensions
{
    public static class CollectionExtension
    {
        public static IEnumerable<object> WithOutEntityNavigation<T>(this IEnumerable<T> collection)
            where T : class,new()
        {
            var listResult=new List<object>();

            foreach (var element in collection)
            {
                var props= element.GetType().GetProperties()
                               .Where(pr => pr.GetGetMethod().IsVirtual == false);

                dynamic temp = new ExpandoObject();
                foreach (var prop in props)
                {
                    
                    ((IDictionary<string, object>)temp).Add(prop.Name, prop.GetValue(element));
                }
                
                
                listResult.Add(temp);
            }

            return listResult;
        }

    }
}
