using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    public class DataCacheAdapter : IDataCache
    {
        public object this[string key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<Microsoft.ApplicationServer.Caching.CacheOperationStartedEventArgs> CacheOperationStarted;

        public event EventHandler<Microsoft.ApplicationServer.Caching.CacheOperationCompletedEventArgs> CacheOperationCompleted;

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value, TimeSpan timeout, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, TimeSpan timeout, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion, TimeSpan timeout, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion Put(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheItemVersion oldVersion, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public object Get(string key, out Microsoft.ApplicationServer.Caching.DataCacheItemVersion version)
        {
            throw new NotImplementedException();
        }

        public object Get(string key, string region)
        {
            throw new NotImplementedException();
        }

        public object Get(string key, out Microsoft.ApplicationServer.Caching.DataCacheItemVersion version, string region)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> BulkGet(IEnumerable<string> keys, string region)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key, string region)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key, Microsoft.ApplicationServer.Caching.DataCacheItemVersion version)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key, Microsoft.ApplicationServer.Caching.DataCacheItemVersion version, string region)
        {
            throw new NotImplementedException();
        }

        public void ResetObjectTimeout(string key, TimeSpan newTimeout)
        {
            throw new NotImplementedException();
        }

        public void ResetObjectTimeout(string key, TimeSpan newTimeout, string region)
        {
            throw new NotImplementedException();
        }

        public object GetIfNewer(string key, ref Microsoft.ApplicationServer.Caching.DataCacheItemVersion version)
        {
            throw new NotImplementedException();
        }

        public object GetIfNewer(string key, ref Microsoft.ApplicationServer.Caching.DataCacheItemVersion version, string region)
        {
            throw new NotImplementedException();
        }

        public object GetAndLock(string key, TimeSpan timeout, out Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle)
        {
            throw new NotImplementedException();
        }

        public object GetAndLock(string key, TimeSpan timeout, out Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, bool forceLock)
        {
            throw new NotImplementedException();
        }

        public object GetAndLock(string key, TimeSpan timeout, out Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, string region, bool forceLock)
        {
            throw new NotImplementedException();
        }

        public object GetAndLock(string key, TimeSpan timeout, out Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, TimeSpan timeout, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItemVersion PutAndUnlock(string key, object value, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public void Unlock(string key, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle)
        {
            throw new NotImplementedException();
        }

        public void Unlock(string key, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void Unlock(string key, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, string region)
        {
            throw new NotImplementedException();
        }

        public void Unlock(string key, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, TimeSpan timeout, string region)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key, Microsoft.ApplicationServer.Caching.DataCacheLockHandle lockHandle, string region)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItem GetCacheItem(string key)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheItem GetCacheItem(string key, string region)
        {
            throw new NotImplementedException();
        }

        public bool CreateRegion(string region)
        {
            throw new NotImplementedException();
        }

        public bool RemoveRegion(string region)
        {
            throw new NotImplementedException();
        }

        public void ClearRegion(string region)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsInRegion(string region)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsByAnyTag(IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsByAllTags(IEnumerable<Microsoft.ApplicationServer.Caching.DataCacheTag> tags, string region)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsByTag(Microsoft.ApplicationServer.Caching.DataCacheTag tag, string region)
        {
            throw new NotImplementedException();
        }

        public string GetSystemRegionName(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSystemRegions()
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheNotificationDescriptor AddCacheLevelCallback(Microsoft.ApplicationServer.Caching.DataCacheOperations filter, Microsoft.ApplicationServer.Caching.DataCacheNotificationCallback clientCallback)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheNotificationDescriptor AddCacheLevelBulkCallback(Microsoft.ApplicationServer.Caching.DataCacheBulkNotificationCallback clientCallback)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheNotificationDescriptor AddRegionLevelCallback(string region, Microsoft.ApplicationServer.Caching.DataCacheOperations filter, Microsoft.ApplicationServer.Caching.DataCacheNotificationCallback clientCallback)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheNotificationDescriptor AddItemLevelCallback(string key, Microsoft.ApplicationServer.Caching.DataCacheOperations filter, Microsoft.ApplicationServer.Caching.DataCacheNotificationCallback clientCallback)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheNotificationDescriptor AddItemLevelCallback(string key, Microsoft.ApplicationServer.Caching.DataCacheOperations filter, Microsoft.ApplicationServer.Caching.DataCacheNotificationCallback clientCallback, string region)
        {
            throw new NotImplementedException();
        }

        public void RemoveCallback(Microsoft.ApplicationServer.Caching.DataCacheNotificationDescriptor nd)
        {
            throw new NotImplementedException();
        }

        public Microsoft.ApplicationServer.Caching.DataCacheNotificationDescriptor AddFailureNotificationCallback(Microsoft.ApplicationServer.Caching.DataCacheFailureNotificationCallback failureCallback)
        {
            throw new NotImplementedException();
        }
    }
}
