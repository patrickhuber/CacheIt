using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace CacheIt
{
    public static class ObjectCacheExtensions
    {
        /// <summary>
        /// Gets an item in the cache. If the item does not exist, the item is placed in the cache.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectCache">The data cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="resolver">The resolver.</param>
        /// <param name="region">The region.</param>
        /// <returns></returns>
        public static T Get<T>(this ObjectCache objectCache, string key, Func<T> resolver, string region = null)
        {

            T value = default(T);
            var cacheItem = string.IsNullOrWhiteSpace(region)
                ? objectCache.GetCacheItem(key)
                : objectCache.GetCacheItem(key, region);

            if (cacheItem == null)
            {
                value = resolver();
                objectCache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration, region);
            }
            else
            {
                value = (T)cacheItem.Value;
            }
            return value;
        }

        /// <summary>
        /// Sets the specified object cache.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="region">The region.</param>
        public static void Set(this ObjectCache objectCache, string key, object value, string region=null)
        {
            objectCache.Set(key, value, ObjectCache.InfiniteAbsoluteExpiration, region);
        }
    }
}
