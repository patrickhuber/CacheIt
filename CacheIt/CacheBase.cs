using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using CacheIt.Diagnostics;
using System.Collections;

namespace CacheIt
{
    /// <summary>
    /// Defines an abstract cache base class with common operations implemented for the rest of the library.
    /// </summary>
    public abstract class CacheBase
        : ObjectCache, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public CacheBase(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheBase"/> class.
        /// </summary>
        public CacheBase()
        { 
        }

        /// <summary>
        /// The name
        /// </summary>
        protected string name;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public override string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Checks whether the cache entry already exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName">Optional. A named region in the cache where the cache can be found, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// true if the cache contains a cache entry with the same key value as <paramref name="key" />; otherwise, false.
        /// </returns>
        public override bool Contains(string key, string regionName = null)
        {
            return GetCacheItem(key, regionName) != null;
        }

        /// <summary>
        /// Asserts the key is valid.
        /// </summary>
        /// <param name="key">The key.</param>
        protected virtual void AssertKeyIsValid(string key)
        {
            Assert.IsNotNullOrWhitespace(key, Strings.InvalidCacheKeyMessage);
        }

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache, specifying a key and a value for the cache entry, and information about how the entry will be evicted.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.
        /// </returns>
        public override object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            if (Contains(key, regionName))
            {
                return Get(key, regionName);
            }
            Add(key, value, policy, regionName);
            return value;
        }
        
        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache, by using a key, an object for the cache entry, an absolute expiration value, and an optional region to add the cache into.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// If a cache entry with the same key exists, the specified cache entry's value; otherwise, null.
        /// </returns>
        public override object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = absoluteExpiration;
            return AddOrGetExisting(key, value, policy, regionName);
        }

        /// <summary>
        /// When overridden in a derived class, gets the specified cache entry from the cache as an object.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// The cache entry that is identified by <paramref name="key" />.
        /// </returns>
        public override object Get(string key, string regionName = null)
        {
            AssertRegionNameIsSupported(regionName);
            if (key == null)
                throw new ArgumentNullException("key");
            var cacheItem = GetCacheItem(key, regionName);
            if (cacheItem == null)
                return null;
            return cacheItem.Value;
        }

        /// <summary>
        /// Gets a set of cache entries that correspond to the specified keys.
        /// </summary>
        /// <param name="keys">A collection of unique identifiers for the cache entries to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry or entries were added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// A dictionary of key/value pairs that represent cache entries.
        /// </returns>
        public override IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (var key in keys)
            {
                var result = this.GetCacheItem(key);
                if (result != null)
                {
                    dictionary.Add(key, result.Value);
                }
            }
            return dictionary;
        }

        /// <summary>
        /// Asserts the region name is supported.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        protected void AssertRegionNameIsSupported(string regionName)
        {
            if ((DefaultCacheCapabilities & System.Runtime.Caching.DefaultCacheCapabilities.CacheRegions) != 0)
                throw new NotSupportedException(Strings.RegionNameNotSupported);            
        }

        /// <summary>
        /// Asserts the change monitor is supported.
        /// </summary>
        /// <exception cref="System.NotSupportedException"></exception>
        protected void AssertChangeMonitorIsSupported()
        {
            if ((DefaultCacheCapabilities & DefaultCacheCapabilities.CacheEntryChangeMonitors) != 0)
                throw new NotSupportedException(Strings.ChangeMonitorsNotSupported);
        }

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
        public override void Set(string key, object value, System.Runtime.Caching.CacheItemPolicy policy, string regionName = null)
        {
            Set(new CacheItem(key, value, regionName), policy);
        }

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache, specifying time-based expiration details.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
        public override void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null)
        {
            Set(new CacheItem(key, value, regionName), new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration });
        }

        /// <summary>
        /// Gets or sets the default indexer for the <see cref="T:System.Runtime.Caching.ObjectCache" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public override object this[string key]
        {
            get
            {
                return Get(key, null);
            }
            set
            {
                Set(key, value, InfiniteAbsoluteExpiration);
            }
        }

        protected bool SupportsCapability(DefaultCacheCapabilities capability)
        {
            return (this.DefaultCacheCapabilities & capability) != 0;
        }
    }
}
