using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Memcached
{
    public class MemcachedCache : CacheBase
    {
        public override System.Runtime.Caching.CacheItem AddOrGetExisting(System.Runtime.Caching.CacheItem value, System.Runtime.Caching.CacheItemPolicy policy)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override System.Runtime.Caching.CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override System.Runtime.Caching.DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { throw new NotImplementedException(); }
        }

        public override System.Runtime.Caching.CacheItem GetCacheItem(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override long GetCount(string regionName = null)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override object Remove(string key, string regionName = null)
        {
            throw new NotImplementedException();
        }

        public override void Set(System.Runtime.Caching.CacheItem item, System.Runtime.Caching.CacheItemPolicy policy)
        {
            throw new NotImplementedException();
        }
    }
}
