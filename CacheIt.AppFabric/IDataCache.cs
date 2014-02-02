using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    /// <summary>
    /// Provides a interface to <see cref="Microsoft.ApplicationServer.Caching.DataCache">DataCache</see> to assist with unit testing
    /// </summary>
    public interface IDataCache
    {
        object this[string key] { get; set; }
        event EventHandler<CacheOperationStartedEventArgs> CacheOperationStarted;
        event EventHandler<CacheOperationCompletedEventArgs> CacheOperationCompleted;
        DataCacheItemVersion Add(string key, object value);
        DataCacheItemVersion Add(string key, object value, string region);
        DataCacheItemVersion Add(string key, object value, TimeSpan timeout);
        DataCacheItemVersion Add(string key, object value, TimeSpan timeout, string region);
        DataCacheItemVersion Add(string key, object value, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion Add(string key, object value, IEnumerable<DataCacheTag> tags, string region);
        DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region);
        DataCacheItemVersion Put(string key, object value);
        DataCacheItemVersion Put(string key, object value, string region);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, string region);
        DataCacheItemVersion Put(string key, object value, TimeSpan timeout);
        DataCacheItemVersion Put(string key, object value, TimeSpan timeout, string region);
        DataCacheItemVersion Put(string key, object value, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion Put(string key, object value, IEnumerable<DataCacheTag> tags, string region);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout, string region);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, IEnumerable<DataCacheTag> tags, string region);
        DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region);
        object Get(string key);
        object Get(string key, out DataCacheItemVersion version);
        object Get(string key, string region);
        object Get(string key, out DataCacheItemVersion version, string region);
        IEnumerable<KeyValuePair<string, object>> BulkGet(IEnumerable<string> keys, string region);
        bool Remove(string key);
        bool Remove(string key, string region);
        bool Remove(string key, DataCacheItemVersion version);
        bool Remove(string key, DataCacheItemVersion version, string region);
        void ResetObjectTimeout(string key, TimeSpan newTimeout);
        void ResetObjectTimeout(string key, TimeSpan newTimeout, string region);
        object GetIfNewer(string key, ref DataCacheItemVersion version);
        object GetIfNewer(string key, ref DataCacheItemVersion version, string region);
        object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle);
        object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock);
        object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, string region, bool forceLock);
        object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, string region);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, string region);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, string region);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, IEnumerable<DataCacheTag> tags, string region);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<DataCacheTag> tags);
        DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region);
        void Unlock(string key, DataCacheLockHandle lockHandle);
        void Unlock(string key, DataCacheLockHandle lockHandle, TimeSpan timeout);
        void Unlock(string key, DataCacheLockHandle lockHandle, string region);
        void Unlock(string key, DataCacheLockHandle lockHandle, TimeSpan timeout, string region);
        bool Remove(string key, DataCacheLockHandle lockHandle);
        bool Remove(string key, DataCacheLockHandle lockHandle, string region);
        DataCacheItem GetCacheItem(string key);
        DataCacheItem GetCacheItem(string key, string region);
        bool CreateRegion(string region);
        bool RemoveRegion(string region);
        void ClearRegion(string region);
        IEnumerable<KeyValuePair<string, object>> GetObjectsInRegion(string region);
        IEnumerable<KeyValuePair<string, object>> GetObjectsByAnyTag(IEnumerable<DataCacheTag> tags, string region);
        IEnumerable<KeyValuePair<string, object>> GetObjectsByAllTags(IEnumerable<DataCacheTag> tags, string region);
        IEnumerable<KeyValuePair<string, object>> GetObjectsByTag(DataCacheTag tag, string region);
        string GetSystemRegionName(string key);
        IEnumerable<string> GetSystemRegions();
        DataCacheNotificationDescriptor AddCacheLevelCallback(DataCacheOperations filter, DataCacheNotificationCallback clientCallback);
        DataCacheNotificationDescriptor AddCacheLevelBulkCallback(DataCacheBulkNotificationCallback clientCallback);
        DataCacheNotificationDescriptor AddRegionLevelCallback(string region, DataCacheOperations filter, DataCacheNotificationCallback clientCallback);
        DataCacheNotificationDescriptor AddItemLevelCallback(string key, DataCacheOperations filter, DataCacheNotificationCallback clientCallback);
        DataCacheNotificationDescriptor AddItemLevelCallback(string key, DataCacheOperations filter, DataCacheNotificationCallback clientCallback, string region);
        void RemoveCallback(DataCacheNotificationDescriptor nd);
        DataCacheNotificationDescriptor AddFailureNotificationCallback(DataCacheFailureNotificationCallback failureCallback);
    }
}
