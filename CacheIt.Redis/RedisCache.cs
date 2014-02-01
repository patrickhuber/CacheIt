using ServiceStack.Caching;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.Redis
{
    public class RedisCache : CacheBase
    {
        private IRedisClient redisClient;
        public RedisCache(IRedisClient cache)
        {
            redisClient = cache;
        }

        public override bool Contains(string key, string regionName = null)
        {
            return redisClient.Get<object>(key) != null;
        }

        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            if (!SupportsCapability(DefaultCacheCapabilities.CacheEntryChangeMonitors))
                throw new NotSupportedException("Change Monitors are not supported.");
            return null;
        }

        public override DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get
            {
                return DefaultCacheCapabilities.AbsoluteExpirations
                    & DefaultCacheCapabilities.OutOfProcessProvider
                    & DefaultCacheCapabilities.SlidingExpirations;
            }
        }

        public override CacheItem GetCacheItem(string key, string regionName = null)
        {            
            return redisClient.Get<CacheItem>(key);
        }

        public override long GetCount(string regionName = null)
        {
            return redisClient.GetAllKeys().Count;
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var key in redisClient.GetAllKeys())
            {
                yield return new KeyValuePair<string, object>(key, redisClient.Get<object>(key));
            }
        }

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            return redisClient.GetAll<object>(keys);
        }

        public override object Remove(string key, string regionName = null)
        {
            var removed = this.Get(key, regionName);
            redisClient.Remove(key);
            return removed;
        }

        public override void Set(CacheItem item, CacheItemPolicy policy)
        {
            redisClient.Set<CacheItem>(item.Key, item, policy.AbsoluteExpiration.DateTime);
        }
    }
}
