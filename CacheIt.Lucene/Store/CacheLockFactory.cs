using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using System.Runtime.Caching;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Creates a cache lock factory used to help maintain index integrity.
    /// </summary>
    public class CacheLockFactory : LockFactory
    {
        private string key;
        private string region;
        private ObjectCache objectCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheLockFactory"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public CacheLockFactory(ObjectCache objectCache, string key, string region)
        {
            this.key = key;
            this.region = region;
            this.objectCache = objectCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheLockFactory"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        public CacheLockFactory(ObjectCache objectCache, string key)
            : this(objectCache, key, null)
        {
        }

        /// <summary>
        /// Attempt to clear (forcefully unlock and remove) the
        /// specified lock.  Only call this at a time when you are
        /// certain this lock is no longer in use.
        /// </summary>
        /// <param name="lockName">name of the lock to be cleared.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void ClearLock(string lockName)
        {
            string lockKey = GenerateLockKey(lockName);
            if (objectCache.Contains(lockKey))
                objectCache.Remove(lockKey);
        }

        /// <summary>
        /// Return a new Lock instance identified by lockName.
        /// </summary>
        /// <param name="lockName">name of the lock to be created.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override Lock MakeLock(string lockName)
        {
            return new CacheLock(
                this.objectCache, 
                GenerateLockKey(lockName), 
                region);
        }

        /// <summary>
        /// Generates the lock key.
        /// </summary>
        /// <param name="lockName">Name of the lock.</param>
        /// <returns></returns>
        private string GenerateLockKey(string lockName)
        {
            return string.Format("{0}-{1}", this.key, lockName);
        }
    }
}
