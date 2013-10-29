using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using System.Runtime.Caching;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Implements a simple locking mechanism for caches
    /// </summary>
    public class CacheLock : Lock
    {
        private string key;
        private string region;
        private ObjectCache objectCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheLock"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        public CacheLock(ObjectCache objectCache, string key)
            : this(objectCache, key, null)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheLock"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        public CacheLock(ObjectCache objectCache, string key, string region)
        {
            this.objectCache = objectCache;
            this.key = key;
            this.region = region;
        }

        /// <summary>
        /// Returns true if the resource is currently locked.  Note that one must
        /// still call <see cref="M:Lucene.Net.Store.Lock.Obtain" /> before using the resource.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool IsLocked()
        {
            return this.objectCache.Contains(key, region);
        }

        /// <summary>
        /// Attempts to obtain exclusive access and immediately return
        /// upon success or failure.
        /// </summary>
        /// <returns>
        /// true iff exclusive access is obtained
        /// </returns>
        public override bool Obtain()
        {
            if (this.objectCache.Contains(key, region))
                return false;
            this.objectCache.Set(key, true, region);
            return true;
        }

        /// <summary>
        /// Releases exclusive access.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Release()
        {            
            if (this.objectCache.Contains(key, region))
            {
                this.objectCache.Remove(key, region);
            }            
        }
    }
}
