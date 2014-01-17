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

        public bool Add(string key, object value, CacheItemPolicy policy, string regionName = null)
        {
            return this.objectCache.Add(key, value, policy, regionName);
        }

        public bool Add(CacheItem item, CacheItemPolicy policy)
        {
            return this.objectCache.Add(item, policy);
        }
    }
}
