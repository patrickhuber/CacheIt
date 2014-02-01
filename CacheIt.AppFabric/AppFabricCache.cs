using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.ApplicationServer.Caching;
using System.Runtime.Caching;
using System.Collections.ObjectModel;

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
        public override CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null)
        {
            return new AppFabricChangeMonitor(
                new ReadOnlyCollection<string>(keys.ToList()), 
                regionName, 
                this.cache);
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

        /// <summary>
        /// Gets the specified cache entry from the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache was added, if regions are implemented. Because regions are not implemented in .NET Framework 4, the default is null.</param>
        /// <returns>
        /// The cache entry that is identified by <paramref name="key" />.
        /// </returns>
        public override CacheItem GetCacheItem(string key, string regionName = null)
        {   
            if (regionName == null)
                return cache.Get(key) as CacheItem;
            
            return cache.Get(key, regionName) as CacheItem;            
        }

        /// <summary>
        /// Gets the total number of cache entries in the cache. 
        /// <see cref="http://stackoverflow.com/questions/4226321/appfabric-get-named-cache-object-count"/>
        /// </summary>
        /// <param name="regionName">Optional. A named region in the cache for which the cache entry count should be computed, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// The number of cache entries in the cache. If <paramref name="regionName" /> is not null, the count indicates the number of entries that are in the specified cache region.
        /// </returns>
        public override long GetCount(string regionName = null)
        {
            if(regionName != null)
            {   
                var objectsInRegion = cache.GetObjectsInRegion(regionName);
                return objectsInRegion.Count();
            }
                        
            long totalItemCount = 0;
            foreach (string systemRegionName in cache.GetSystemRegions())
            {
                totalItemCount += cache.GetObjectsInRegion(regionName).Count();
            }
            return totalItemCount;
        }

        /// <summary>
        /// Creates an enumerator that can be used to iterate through a collection of cache entries.
        /// </summary>
        /// <returns>
        /// The enumerator object that provides access to the cache entries in the cache.
        /// </returns>
        protected override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (string regionName in cache.GetSystemRegions())
            {
                foreach (var keyValuePair in cache.GetObjectsInRegion(regionName))
                {
                    yield return keyValuePair;
                }
            }            
        }

        /// <summary>
        /// Gets a set of cache entries that correspond to the specified keys.
        /// </summary>
        /// <param name="keys">A collection of unique identifiers for the cache entries to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry or entries were added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// A dictionary of key/value pairs that represent cache entries.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            // perhaps add some code to check for object size?
            // > 5k causes performance problems
            return cache
                .BulkGet(keys, regionName)
                .ToDictionary(x=>x.Key, x=>x.Value);
        }

        /// <summary>
        /// Removes the cache entry from the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// An object that represents the value of the removed cache entry that was specified by the key, or null if the specified entry was not found.
        /// </returns>
        public override object Remove(string key, string regionName = null)
        {
            var instance = Get(key, regionName);
            if (regionName == null)
                cache.Remove(key);
            else
                cache.Remove(key, regionName);
            return instance;
        }

        /// <summary>
        /// Inserts the cache entry into the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance, specifying information about how the entry will be evicted.
        /// </summary>
        /// <param name="item">The cache item to add.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        public override void Set(CacheItem item, CacheItemPolicy policy)
        {   
            if (policy.AbsoluteExpiration != ObjectCache.InfiniteAbsoluteExpiration)
            {
                if (item.RegionName == null)
                    cache.Put(item.Key, item, policy.AbsoluteExpiration - DateTime.Now);
                else
                    cache.Put(item.Key, item, policy.AbsoluteExpiration - DateTime.Now, item.RegionName);            
            }
            else
            {
                if (item.RegionName == null)
                    cache.Put(item.Key, item);
                else
                    cache.Put(item.Key, item, item.RegionName);   
            }            
        }

        public override CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy)
        {
            throw new NotImplementedException();
        }
    }
}
