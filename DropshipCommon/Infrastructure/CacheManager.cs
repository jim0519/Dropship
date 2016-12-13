using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;

namespace DropshipCommon.Infrastructure
{
    /// <summary>
    /// Cache manager interface
    /// </summary>
    public interface ICacheManager
    {
        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        bool IsSet(string key);

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        void Remove(string key);

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// Clear all cache data
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Represents a manager for caching between HTTP requests (long term caching)
    /// </summary>
    public partial class CacheManager : ICacheManager
    {
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public virtual T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="data">Data</param>
        /// <param name="cacheTime">Cache time</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            if(cacheTime>0)
                policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        /// Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>Result</returns>
        public virtual bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key">/key</param>
        public virtual void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public virtual void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (var item in Cache)
                if (regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// Clear all cache data
        /// </summary>
        public virtual void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }



    /// <summary>
    /// Extensions
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Variable (lock) to support thread-safe
        /// </summary>
        private static readonly object _syncObject = new object();

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static T GetForOneHour<T>(this ICacheManager cacheManager, string key, Func<T> acquire)
        {
            return Get(cacheManager, key, acquire, 60);
        }

        /// <summary>
        /// Get a cached item. If it's not in the cache yet, then load and cache it
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="key">Cache key</param>
        /// <param name="cacheTime">Cache time in minutes (0 - do not cache)</param>
        /// <param name="acquire">Function to load item if it's not in the cache yet</param>
        /// <returns>Cached item</returns>
        public static T Get<T>(this ICacheManager cacheManager, string key, Func<T> acquire, int cacheTime = 0)
        {
            lock (_syncObject)
            {
                if (cacheManager.IsSet(key))
                {
                    return cacheManager.Get<T>(key);
                }

                var result = acquire();
                //if (cacheTime > 0)
                cacheManager.Set(key, result, cacheTime);
                return result;
            }
        }
    }

    //#region CacheFunc

    //public class CacheFunc
    //{
    //    public static Func<IList<string>> GetItemStatusList = () =>
    //    {
    //        return new List<string>() { "ECM", "ECV", "CVMAGENTO", "CMNOP", "MANUAL", "RESEND", "FAULTY" };

    //    };

    //    public static Func<IList<string>> GetListingStatusList = () =>
    //    {
    //        return new List<string>() { "CREATED", "READY", "SHIPPED", "CANCELLED" };
    //    };

    //    public static Func<IList<string>> GetSupplierList = () =>
    //    {
    //        return new List<string>() { "UNPAID", "PAID", "REFUND" };
    //    };

    //    public static Func<IList<string>> GetListingChannelList = () =>
    //    {
    //        return new List<string>() { "UNPAID", "PAID", "REFUND" };
    //    };
    //}

    //#endregion

}
