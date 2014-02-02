using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt
{
    /// <summary>
    /// Represents an interface of ObjectCache
    /// </summary>
    public interface IObjectCache : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        /// <summary>
        /// When overridden in a derived class, gets a description of the features that a cache implementation provides.
        /// </summary>
        /// <returns>A bitwise combination of flags that indicate the default capabilities of a cache implementation.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        DefaultCacheCapabilities DefaultCacheCapabilities { get; }

        /// <summary>
        /// Gets the name of a specific <see cref="T:System.Runtime.Caching.ObjectCache" /> instance.
        /// </summary>
        /// <returns>The name of a specific cache instance.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        string Name { get; }

        /// <summary>
        /// Gets or sets the default indexer for the <see cref="T:System.Runtime.Caching.ObjectCache" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        object this[string key] { get; set; }

        /// <summary>
        /// When overridden in a derived class, creates a <see cref="T:System.Runtime.Caching.CacheEntryChangeMonitor" /> object that can trigger events in response to changes to specified cache entries.
        /// </summary>
        /// <param name="keys">The unique identifiers for cache entries to monitor.</param>
        /// <param name="regionName">Optional. A named region in the cache where the cache keys in the <paramref name="keys" /> parameter exist, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// A change monitor that monitors cache entries in the cache.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        CacheEntryChangeMonitor CreateCacheEntryChangeMonitor(IEnumerable<string> keys, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, checks whether the cache entry already exists in the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName">Optional. A named region in the cache where the cache can be found, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// true if the cache contains a cache entry with the same key value as <paramref name="key" />; otherwise, false.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        bool Contains(string key, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache without overwriting any existing cache entry.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire. This parameter is required when the <see cref="Overload:System.Runtime.Caching.ObjectCache.Add" /> method is called.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. Because regions are not implemented in .NET Framework 4, the default value is null.</param>
        /// <returns>
        /// true if insertion succeeded, or false if there is an already an entry in the cache that has the same key as <paramref name="key" />.
        /// </returns>
        bool Add(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, tries to insert a cache entry into the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance, and adds details about how the entry should be evicted.
        /// </summary>
        /// <param name="item">The object to add.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <returns>
        /// true if insertion succeeded, or false if there is an already an entry in the cache that has the same key as <paramref name="item" />.
        /// </returns>
        bool Add(CacheItem item, CacheItemPolicy policy);

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache, specifying information about how the entry will be evicted.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// true if the insertion try succeeds, or false if there is an already an entry in the cache with the same key as <paramref name="key" />.
        /// </returns>
        bool Add(string key, object value, CacheItemPolicy policy, string regionName = null);

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
        /// <exception cref="System.NotImplementedException"></exception>
        object AddOrGetExisting(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, inserts the specified <see cref="T:System.Runtime.Caching.CacheItem" /> object into the cache, specifying information about how the entry will be evicted.
        /// </summary>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <returns>
        /// If a cache entry with the same key exists, the specified cache entry; otherwise, null.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        CacheItem AddOrGetExisting(CacheItem value, CacheItemPolicy policy);

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
        /// <exception cref="System.NotImplementedException"></exception>
        object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null);
        
        /// <summary>
        /// When overridden in a derived class, gets the specified cache entry from the cache as an object.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// The cache entry that is identified by <paramref name="key" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        object Get(string key, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, gets the specified cache entry from the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache was added, if regions are implemented. Because regions are not implemented in .NET Framework 4, the default is null.</param>
        /// <returns>
        /// The cache entry that is identified by <paramref name="key" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        CacheItem GetCacheItem(string key, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache, specifying time-based expiration details.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="absoluteExpiration">The fixed date and time at which the cache entry will expire.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void Set(string key, object value, DateTimeOffset absoluteExpiration, string regionName = null);
        
        /// <summary>
        /// When overridden in a derived class, inserts the cache entry into the cache as a <see cref="T:System.Runtime.Caching.CacheItem" /> instance, specifying information about how the entry will be evicted.
        /// </summary>
        /// <param name="item">The cache item to add.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void Set(CacheItem item, CacheItemPolicy policy);

        /// <summary>
        /// When overridden in a derived class, inserts a cache entry into the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="value">The object to insert.</param>
        /// <param name="policy">An object that contains eviction details for the cache entry. This object provides more options for eviction than a simple absolute expiration.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry can be added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void Set(string key, object value, CacheItemPolicy policy, string regionName = null);
        
        /// <summary>
        /// When overridden in a derived class, gets a set of cache entries that correspond to the specified keys.
        /// </summary>
        /// <param name="keys">A collection of unique identifiers for the cache entries to get.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry or entries were added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// A dictionary of key/value pairs that represent cache entries.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        IDictionary<string, object> GetValues(IEnumerable<string> keys, string regionName = null);

        /// <summary>
        /// Gets a set of cache entries that correspond to the specified keys.
        /// </summary>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry or entries were added, if regions are implemented. Because regions are not implemented in .NET Framework 4, the default is null.</param>
        /// <param name="keys">A collection of unique identifiers for the cache entries to get.</param>
        /// <returns>
        /// A dictionary of key/value pairs that represent cache entries.
        /// </returns>
        IDictionary<string, object> GetValues(string regionName, params string[] keys);

        /// <summary>
        /// When overridden in a derived class, removes the cache entry from the cache.
        /// </summary>
        /// <param name="key">A unique identifier for the cache entry.</param>
        /// <param name="regionName">Optional. A named region in the cache to which the cache entry was added, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// An object that represents the value of the removed cache entry that was specified by the key, or null if the specified entry was not found.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        object Remove(string key, string regionName = null);

        /// <summary>
        /// When overridden in a derived class, gets the total number of cache entries in the cache.
        /// </summary>
        /// <param name="regionName">Optional. A named region in the cache for which the cache entry count should be computed, if regions are implemented. The default value for the optional parameter is null.</param>
        /// <returns>
        /// The number of cache entries in the cache. If <paramref name="regionName" /> is not null, the count indicates the number of entries that are in the specified cache region.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        long GetCount(string regionName = null);
    }
}
