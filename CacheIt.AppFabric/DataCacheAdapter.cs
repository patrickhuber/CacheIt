using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    public class DataCacheAdapter : IDataCache
    {
        private DataCache dataCache;
        public DataCacheAdapter(DataCache dataCache)
        {
            this.dataCache = dataCache;
        }

        public object this[string key]
        {
            get { return this.dataCache[key]; }
            set { this.dataCache[key] = value; }
        }

        public event EventHandler<CacheOperationStartedEventArgs> CacheOperationStarted
        {
            add 
            {
                this.dataCache.CacheOperationStarted += value;
            }
            remove 
            {
                this.dataCache.CacheOperationStarted -= value;
            }
        }

        public event EventHandler<CacheOperationCompletedEventArgs> CacheOperationCompleted
        {
            add 
            {
                this.dataCache.CacheOperationCompleted += value;
            }
            remove 
            {
                this.dataCache.CacheOperationCompleted -= value;
            }
        }

        public DataCacheItemVersion Add(string key, object value)
        {
            return this.dataCache.Add(key, value);
        }

        public DataCacheItemVersion Add(string key, object value, string region)
        {
            return this.dataCache.Add(key, value, region);
        }

        public DataCacheItemVersion Add(string key, object value, TimeSpan timeout)
        {
            return this.dataCache.Add(key, value, timeout);
        }

        public DataCacheItemVersion Add(string key, object value, TimeSpan timeout, string region)
        {
            return this.dataCache.Add(key, value, timeout, region);
        }

        public DataCacheItemVersion Add(string key, object value, IEnumerable<DataCacheTag> tags)
        {
            return this.dataCache.Add(key, value, tags);
        }

        public DataCacheItemVersion Add(string key, object value, IEnumerable<DataCacheTag> tags, string region)
        {
            return this.dataCache.Add(key, value, tags, region);
        }

        public DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            return this.dataCache.Add(key, value, timeout, tags);
        }

        public DataCacheItemVersion Add(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            return this.dataCache.Add(key, value, timeout, tags, region);
        }

        public DataCacheItemVersion Put(string key, object value)
        {
            return this.dataCache.Put(key, value);
        }

        public DataCacheItemVersion Put(string key, object value, string region)
        {
            return this.dataCache.Put(key, value, region);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion)
        {
            return this.dataCache.Put(key, value, oldVersion);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, string region)
        {
            return this.dataCache.Put(key, value, oldVersion, region);
        }

        public DataCacheItemVersion Put(string key, object value, TimeSpan timeout)
        {
            return this.dataCache.Put(key, value, timeout);
        }

        public DataCacheItemVersion Put(string key, object value, TimeSpan timeout, string region)
        {
            return this.dataCache.Put(key, value, timeout, region);
        }

        public DataCacheItemVersion Put(string key, object value, IEnumerable<DataCacheTag> tags)
        {
            return this.dataCache.Put(key, value, tags);
        }

        public DataCacheItemVersion Put(string key, object value, IEnumerable<DataCacheTag> tags, string region)
        {
            return this.dataCache.Put(key, value, region);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout)
        {
            return this.dataCache.Put(key, value, oldVersion, timeout);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout, string region)
        {
            return this.dataCache.Put(key, value, oldVersion, timeout, region);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, IEnumerable<DataCacheTag> tags)
        {
            return this.dataCache.Put(key, value, oldVersion, tags);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, IEnumerable<DataCacheTag> tags, string region)
        {
            return this.dataCache.Put(key, value, oldVersion, tags, region);
        }

        public DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            return this.dataCache.Put(key, value, timeout, tags);
        }

        public DataCacheItemVersion Put(string key, object value, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            return this.dataCache.Put(key, value, timeout, tags, region);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            return this.dataCache.Put(key, value, oldVersion, timeout, tags);
        }

        public DataCacheItemVersion Put(string key, object value, DataCacheItemVersion oldVersion, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            return this.dataCache.Put(key, value, oldVersion, timeout, tags, region);
        }

        public object Get(string key)
        {
            return dataCache.Get(key);
        }

        public object Get(string key, out DataCacheItemVersion version)
        {
            return dataCache.Get(key, out version);
        }

        public object Get(string key, string region)
        {
            return dataCache.Get(key, region);
        }

        public object Get(string key, out DataCacheItemVersion version, string region)
        {
            return dataCache.Get(key, out version, region);
        }

        public IEnumerable<KeyValuePair<string, object>> BulkGet(IEnumerable<string> keys, string region)
        {
            return dataCache.BulkGet(keys, region);
        }

        public bool Remove(string key)
        {
            return dataCache.Remove(key);
        }

        public bool Remove(string key, string region)
        {
            return dataCache.Remove(key, region);
        }

        public bool Remove(string key, DataCacheItemVersion version)
        {
            return dataCache.Remove(key, version);
        }

        public bool Remove(string key, DataCacheItemVersion version, string region)
        {
            return dataCache.Remove(key, version, region);
        }

        public void ResetObjectTimeout(string key, TimeSpan newTimeout)
        {
            dataCache.ResetObjectTimeout(key, newTimeout);
        }

        public void ResetObjectTimeout(string key, TimeSpan newTimeout, string region)
        {
            dataCache.ResetObjectTimeout(key, newTimeout, region);
        }

        public object GetIfNewer(string key, ref DataCacheItemVersion version)
        {
            return dataCache.GetIfNewer(key, ref version);
        }

        public object GetIfNewer(string key, ref DataCacheItemVersion version, string region)
        {
            return dataCache.GetIfNewer(key, ref version, region);
        }

        public object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle)
        {
            return dataCache.GetAndLock(key, timeout, out lockHandle);
        }

        public object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, bool forceLock)
        {
            return dataCache.GetAndLock(key, timeout, out lockHandle, forceLock);
        }

        public object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, string region, bool forceLock)
        {
            return dataCache.GetAndLock(key, timeout, out lockHandle, region, forceLock);
        }

        public object GetAndLock(string key, TimeSpan timeout, out DataCacheLockHandle lockHandle, string region)
        {
            return dataCache.GetAndLock(key, timeout, out lockHandle, region);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, string region)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle, region);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle, timeout);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, string region)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle, timeout, region);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, IEnumerable<DataCacheTag> tags)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle, tags);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, IEnumerable<DataCacheTag> tags, string region)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle, tags, region);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<DataCacheTag> tags)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle, timeout, tags);
        }

        public DataCacheItemVersion PutAndUnlock(string key, object value, DataCacheLockHandle lockHandle, TimeSpan timeout, IEnumerable<DataCacheTag> tags, string region)
        {
            return dataCache.PutAndUnlock(key, value, lockHandle, timeout, tags, region);
        }

        public void Unlock(string key, DataCacheLockHandle lockHandle)
        {
            dataCache.Unlock(key, lockHandle);
        }

        public void Unlock(string key, DataCacheLockHandle lockHandle, TimeSpan timeout)
        {
            dataCache.Unlock(key, lockHandle, timeout);
        }

        public void Unlock(string key, DataCacheLockHandle lockHandle, string region)
        {
            dataCache.Unlock(key, lockHandle, region);
        }

        public void Unlock(string key, DataCacheLockHandle lockHandle, TimeSpan timeout, string region)
        {
            dataCache.Unlock(key, lockHandle, timeout, region);
        }

        public bool Remove(string key, DataCacheLockHandle lockHandle)
        {
            return dataCache.Remove(key, lockHandle);
        }

        public bool Remove(string key, DataCacheLockHandle lockHandle, string region)
        {
            return dataCache.Remove(key, lockHandle, region);
        }

        public DataCacheItem GetCacheItem(string key)
        {
            return dataCache.GetCacheItem(key);
        }

        public DataCacheItem GetCacheItem(string key, string region)
        {
            return dataCache.GetCacheItem(key, region);
        }

        public bool CreateRegion(string region)
        {
            return dataCache.CreateRegion(region);
        }

        public bool RemoveRegion(string region)
        {
            return dataCache.RemoveRegion(region);
        }

        public void ClearRegion(string region)
        {
            dataCache.ClearRegion(region);
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsInRegion(string region)
        {
            return dataCache.GetObjectsInRegion(region);
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsByAnyTag(IEnumerable<DataCacheTag> tags, string region)
        {
            return dataCache.GetObjectsByAnyTag(tags, region);
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsByAllTags(IEnumerable<DataCacheTag> tags, string region)
        {
            return dataCache.GetObjectsByAllTags(tags, region);
        }

        public IEnumerable<KeyValuePair<string, object>> GetObjectsByTag(DataCacheTag tag, string region)
        {
            return dataCache.GetObjectsByTag(tag, region);
        }

        public string GetSystemRegionName(string key)
        {
            return dataCache.GetSystemRegionName(key);
        }

        public IEnumerable<string> GetSystemRegions()
        {
            return dataCache.GetSystemRegions();
        }

        public DataCacheNotificationDescriptor AddCacheLevelCallback(DataCacheOperations filter, DataCacheNotificationCallback clientCallback)
        {
            return dataCache.AddCacheLevelCallback(filter, clientCallback);
        }

        public DataCacheNotificationDescriptor AddCacheLevelBulkCallback(DataCacheBulkNotificationCallback clientCallback)
        {
            return dataCache.AddCacheLevelBulkCallback(clientCallback);
        }

        public DataCacheNotificationDescriptor AddRegionLevelCallback(string region, DataCacheOperations filter, DataCacheNotificationCallback clientCallback)
        {
            return dataCache.AddRegionLevelCallback(region, filter, clientCallback);
        }

        public DataCacheNotificationDescriptor AddItemLevelCallback(string key, DataCacheOperations filter, DataCacheNotificationCallback clientCallback)
        {
            return dataCache.AddItemLevelCallback(key, filter, clientCallback);
        }

        public DataCacheNotificationDescriptor AddItemLevelCallback(string key, DataCacheOperations filter, DataCacheNotificationCallback clientCallback, string region)
        {
            return dataCache.AddItemLevelCallback(key, filter, clientCallback, region);
        }

        public void RemoveCallback(DataCacheNotificationDescriptor nd)
        {
            dataCache.RemoveCallback(nd);
        }

        public DataCacheNotificationDescriptor AddFailureNotificationCallback(DataCacheFailureNotificationCallback failureCallback)
        {
            return dataCache.AddFailureNotificationCallback(failureCallback);
        }
    }
}
