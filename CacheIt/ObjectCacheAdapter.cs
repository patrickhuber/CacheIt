using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt
{
    /// <summary>
    /// Provides and adapter of ObjectCache to implement IObjectCache
    /// </summary>
    public class ObjectCacheAdapter : IObjectCache
    {
        /// <summary>
        /// The object cache
        /// </summary>
        private ObjectCache objectCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectCacheAdapter"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        public ObjectCacheAdapter(ObjectCache objectCache)
        {
            this.objectCache = objectCache;
        }

        public DefaultCacheCapabilities DefaultCacheCapabilities
        {
            get { return this.objectCache.DefaultCacheCapabilities; }
        }

        public string Name
        {
            get { return this.objectCache.Name; }
        }

        public object this[string key]
        {
            get { return this.objectCache[key]; }
            set { this.objectCache[key] = value; }
        }

        public CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            return this.objectCache.CreateCacheEntryChangeMonitor(keys, regionName);
        }

        public bool Contains(string key, string regionName = null)
        {
            return this.objectCache.Contains(key, regionName);
        }

        public bool Add(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return this.objectCache.Add(key, value, absoluteExpiration, regionName);
        }

        public bool Add(CacheItem item, CacheItemPolicy policy)
        {
            return this.objectCache.Add(item, policy);
        }

        public bool Add(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            return this.objectCache.Add(key, value, policy, regionName);
        }

        public object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            return this.objectCache.Add(key, value, absoluteExpiration, regionName);
        }

        public CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            return this.objectCache.AddOrGetExisting(value, policy);
        }

        public object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            return this.objectCache.AddOrGetExisting(key, value, policy, regionName);
        }

        public object Get(string key, string regionName = null)
        {
            return this.objectCache.Get(key, regionName);
        }

        public CacheItem GetCacheItem(string key, string regionName = null)
        {
            return this.objectCache.GetCacheItem(key, regionName);
        }

        public void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            this.objectCache.Set(key, value, absoluteExpiration, regionName);
        }

        public void Set(CacheItem item, CacheItemPolicy policy)
        {
            this.objectCache.Set(item, policy);
        }

        public void Set(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            this.objectCache.Set(key, value, policy, regionName);
        }

        public IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            return this.objectCache.GetValues(keys, regionName);
        }

        public IDictionary<string, object> GetValues(string regionName, params string[] keys)
        {
            return this.objectCache.GetValues(regionName, keys);
        }

        public object Remove(string key, string regionName = null)
        {
            return this.objectCache.Remove(key, regionName);
        }

        public long GetCount(string regionName = null)
        {
            return this.objectCache.GetCount(regionName);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var kvp in objectCache)
                yield return kvp;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var kvp in objectCache)
                yield return kvp;
        }
    }
}