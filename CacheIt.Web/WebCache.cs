using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Web.Caching;

namespace CacheIt.Web
{
    public class WebCache : CacheBase
    {
        private Cache cache;

        public WebCache(Cache cache) : base()
        {
            this.cache = cache;
        }
        
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            
            throw new NotImplementedException();
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return DefaultCacheCapabilities.AbsoluteExpirations
                    | DefaultCacheCapabilities.CacheEntryChangeMonitors
                    | DefaultCacheCapabilities.CacheEntryRemovedCallback
                    | DefaultCacheCapabilities.CacheEntryUpdateCallback
                    | DefaultCacheCapabilities.InMemoryProvider
                    | DefaultCacheCapabilities.SlidingExpirations;
            }
        }

        public override CacheItem GetCacheItem(string key, string regionName = null)
        {
            return this.cache.Get(key) as CacheItem;
        }

        public override long GetCount(string regionName = null)
        {
            return this.cache.Count;
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var instance in this.cache)
            {
                var cacheItem = instance as CacheItem;
                yield return new KeyValuePair<string, object>(cacheItem.Key, cacheItem.Value);
            }
        }
        
        public override object Remove(string key, string regionName = null)
        {
            var cacheItem = this.cache.Remove(key) as CacheItem;
            return cacheItem != null ? cacheItem.Value : null;
        }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            Insert(item, policy);
        }

        public event CacheItemRemovedCallback OnCacheItemRemoved = null;

        public System.Web.Caching.CacheItemPriority Map(System.Runtime.Caching.CacheItemPriority priority)
        {
            switch (priority)
            {
                case System.Runtime.Caching.CacheItemPriority.Default:
                    return System.Web.Caching.CacheItemPriority.NotRemovable;

                case System.Runtime.Caching.CacheItemPriority.NotRemovable:
                    return System.Web.Caching.CacheItemPriority.NotRemovable;
                default:
                    return System.Web.Caching.CacheItemPriority.Default;
            }
        }

        public override CacheItem AddOrGetExisting(CacheItem item, CacheItemPolicy policy)
        {
            var instance = GetCacheItem(item.Key);
            if (instance != null)
                return instance;
            return Insert(item, policy);
        }

        private CacheItem Insert(CacheItem cacheItem, CacheItemPolicy policy)
        {
            this.cache.Insert(
                cacheItem.Key,
                cacheItem,
                null,
                policy.AbsoluteExpiration.DateTime,
                policy.SlidingExpiration,
                Map(policy.Priority),
                OnCacheItemRemoved);
            return cacheItem;
        }
    }
}
