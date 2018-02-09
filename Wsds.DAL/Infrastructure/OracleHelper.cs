using System;
using System.Collections.Generic;
using System.Linq;
using Wsds.DAL.UserAttrubutes;

namespace Wsds.DAL.Infrastructure
{
    public static class OracleHelper
    {

        // Возвращает имя секвенса на entity, если таковой прописан в аннотации WsdsOraSequenceName
        //        public static string GetWsdsOraSequenceName<T> (T item) where T : new()
        public static string GetWsdsOraSequenceName<T>(T item) where T : new()
        {
            var seqMapping = item.GetType().GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType() == typeof(WsdsOraSequenceNameAttribute)); 
            if (seqMapping != null)
            {
                return (seqMapping as WsdsOraSequenceNameAttribute).Name;
            }
            else
                return null;
        }
    
    }
}