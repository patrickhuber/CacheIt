using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;
using System.Runtime.Caching;

namespace CacheIt.AppFabric
{
    /// <summary>
    /// Defines an appfabric representation of the CacheBase
    /// </summary>
    public class AppFabricCache : CacheBase
    {
        /// <summary>
        /// The cache
        /// </summary>
        private DataCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFabricCache"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="dataCacheFactory">The data cache factory.</param>
        public AppFabricCache(string name, DataCacheFactory dataCacheFactory)
            : base()
        {
            cache = dataCacheFactory.GetCache(name);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppFabricCache"/> class.
        /// </summary>
        /// <param name="dataCache">The data cache.</param>
        public AppFabricCache(DataCache dataCache)
        {
            cache = dataCache;
        }

        /// <summary>
        /// When overridden in a derived class, checks whether the cache entry already exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName">Optional. A named region in the cache where the cache can be found, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// true if the cache contains a cache entry with the same key value as <paramref name="key" />; otherwise, false.
        /// </returns>
        public override bool Contains(string key, string regionName = null)
        {
            return cache.Get(key, regionName) != null;
        }

        /// <summary>
        /// When overridden in a derived class, creates a <see cref="T:System.Runtime.Caching.CacheEntryChangeMonitor" /> object that can trigger events in response to changes to specified cache entries.
        /// </summary>
        /// <param name="keys">The unique identifiers for cache entries to monitor.</param>
        /// <param name="regionName">Optional. A named region in the cache where the cache keys in the <paramref name="keys" /> parameter exist, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// A change monitor that monitors cache entries in the cache.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override System.Runtime.Caching.CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a description of the features that a cache implementation provides.
        /// </summary>
        /// <returns>A bitwise combination of flags that indicate the default capabilities of a cache implementation.</returns>
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

        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
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
