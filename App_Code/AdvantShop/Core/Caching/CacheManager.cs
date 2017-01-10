//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections;
using System.Web;
using System.Web.Caching;
using System.Collections.Generic;

namespace AdvantShop.Core.Caching
{
    public class CacheManager
    {
        private const int DefaultTime = 2;
        private static readonly Object SyncObject = new Object();

        public static Cache CacheObject = HttpRuntime.Cache;

        public static void Insert<T>(String key, T value)
        {
            Insert(key, value, DefaultTime);
        }

        public static void Insert<T>(String key, T value, int timeSpan)
        {
            lock (SyncObject)
            {
                DateTime expiration = DateTime.Now.AddMinutes(timeSpan);
                CacheObject.Insert(key, value, null, expiration, TimeSpan.Zero, CacheItemPriority.Default, null);
            }
        }

        public static bool Contains(String key)
        {
            return CacheObject.Get(key) != null;
        }

        public static int Count()
        {
            return CacheObject.Count;
        }

        public static T Get<T>(string key)
        {
            var value = CacheObject.Get(key);
            return (value is T) ? ((T)value) : default(T);
        }

        public static void Clean()
        {
            IDictionaryEnumerator enumerator = CacheObject.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CacheObject.Remove(enumerator.Key.ToString());
            }
        }

        public static void Remove(string key)
        {
            CacheObject.Remove(key);
        }

        public static void RemoveByPattern(string key)
        {
            IDictionaryEnumerator item = CacheObject.GetEnumerator();
            while (item.MoveNext())
            {
                if (item.Key.ToString().Contains(key))
                    CacheObject.Remove(item.Key.ToString());
            }
        }

        public static List<T> GetByPattern<T>(string key)
        {
            List<T> list = new List<T>();
            IDictionaryEnumerator item = CacheObject.GetEnumerator();
            while (item.MoveNext())
            {
                if (item.Key.ToString().Contains(key))
                {
                    var value = CacheObject.Get(key);
                    list.Add((value is T) ? ((T)value) : default(T));
                }
            }
            return list;
        }
    }
}