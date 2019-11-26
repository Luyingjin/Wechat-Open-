using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Formula.Helper
{
    public class CacheHelper
    {
        public static object Get(string key, Func<object> fun, int second = 0)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["UseCache"] != "True")
                return fun();
            object obj = HttpRuntime.Cache.Get(key);
            if (obj == null)
            {
                obj = fun();
                if (obj != null)
                {
                    if (second == 0)
                        HttpRuntime.Cache.Insert(key, obj);
                    else
                        HttpRuntime.Cache.Insert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(second));
                }
            }

            return obj;
        }


        public static bool Set(string key, object value)
        {
            if (value == null)
                return false;
            HttpRuntime.Cache.Insert(key, value);
            return true;
        }

        public static bool Set(string key, object value, int second)
        {
            HttpRuntime.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(second));
            return true;
        }

        public static object Get(string key)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["UseCache"] != "True")
                return null;

            return HttpRuntime.Cache.Get(key);
        }

        /// <summary>
        /// 从缓存中获取一组对象
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns>值</returns>
        /// <remarks>如有多个键，请以逗号分隔</remarks>
        public static object[] GetObjects(string keys)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["UseCache"] != "True")
                return null;
            if (string.IsNullOrEmpty(keys))
                return null;
            string[] keyArray = keys.Split(',');
            object[] objs = new object[keyArray.Length];
            for (int i = 0; i < keyArray.Length; i++)
            {
                objs[i] = HttpRuntime.Cache.Get(keyArray[i]);
            }
            return objs;
        }

        public static void Remove(string key)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["UseCache"] != "True")
                return;
            HttpRuntime.Cache.Remove(key);
        }

        #region Memcached

        //static MemcachedClient cache;


        //static CacheHelper()
        //{
        //    MemcachedClient.Setup("MyCache", new string[] { "localhost" });
        //    cache = MemcachedClient.GetInstance("MyCache");
        //    cache.SendReceiveTimeout = 5000;
        //    cache.ConnectTimeout = 5000;
        //    cache.MinPoolSize = 1;
        //    cache.MaxPoolSize = 5;

        //}

        //#region Set

        //public static bool Set(string key, object value)
        //{
        //    return cache.Set(key, value);
        //}

        //public static bool Set(string key, object value, DateTime expiry)
        //{
        //    return cache.Set(key, value, expiry);
        //}

        //public static bool Set(string key, object value, TimeSpan expiry)
        //{
        //    return cache.Set(key, value, expiry);
        //}

        //public static bool Set(string key, object value, uint hash)
        //{
        //    return cache.Set(key, value, hash);
        //}

        //public static bool Set(string key, object value, uint hash, DateTime expiry)
        //{
        //    return cache.Set(key, value, hash, expiry);
        //}

        //public static bool Set(string key, object value, uint hash, TimeSpan expiry)
        //{
        //    return cache.Set(key, value, hash, expiry);
        //}

        //#endregion

        //#region Get

        //public static object Get(string key)
        //{
        //    return cache.Get(key);
        //}

        //public static object[] Get(string[] keys)
        //{
        //    return cache.Get(keys);
        //}

        //public static object Get(string key, uint hash)
        //{
        //    return cache.Get(key, hash);
        //}

        //public static object[] Get(string[] keys, uint[] hashes)
        //{
        //    return cache.Get(keys, hashes);
        //}

        //#endregion

        //#region Gets

        //public static object Gets(string key, out ulong unique)
        //{
        //    return cache.Gets(key, out unique);
        //}

        //public static object[] Gets(string[] keys, out ulong[] uniques)
        //{
        //    return cache.Gets(keys, out uniques);
        //}

        //public static object Gets(string key, uint hash, out ulong unique)
        //{
        //    return cache.Gets(key, hash, out unique);
        //}

        //public static object[] Gets(string[] keys, uint[] hashes, out ulong[] uniques)
        //{
        //    return cache.Gets(keys, hashes, out uniques);
        //}


        //#endregion

        //public static void Remove(string key)
        //{
        //    object obj = cache.Get(key);
        //    if (obj != null)
        //        cache.Delete(key);
        //}


        #endregion
    }

}
