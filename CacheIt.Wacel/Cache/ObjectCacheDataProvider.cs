using Microsoft.Ted.Wacel.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.Wacel.Cache
{
    public class ObjectCacheDataProvider : IDataProvider
    {
        private ObjectCache objectCache;
        private DataProviderConfiguration configuration;
        private string region;

        public ObjectCacheDataProvider(ObjectCache objectCache, string region = null)
        {
            this.objectCache = objectCache;
            this.region = region;
        }

        public IEnumerable<KeyValuePair<string, object>> BulkGet(List<string> keys, string partition = null)
        {
            return objectCache.GetValues(keys, region);
        }

        public void BulkPut(IEnumerable<KeyValuePair<string, object>> values, string partition = null)
        {
            foreach (var keyValuePair in values)
                objectCache.Set(keyValuePair.Key, keyValuePair.Value, ObjectCache.InfiniteAbsoluteExpiration, region);
        }

        public void Clear()
        {
            objectCache.Clear(region);            
        }

        public IDataProvider Clone(string newName)
        {
            return (IDataProvider)new ObjectCacheDataProvider(this.objectCache, newName)
            {
                Configuration = (DataProviderConfiguration)this.configuration.Clone()
            };
        }

        public DataProviderConfiguration Configuration
        {
            get { return this.configuration; }
            set { this.configuration = value; }
        }

        public void Delete(string key, string partition = null)
        {
            objectCache.Remove(key, region);
        }

        public object Get(string key, string partition = null)
        {
            return objectCache.Get(key, region);
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll(string partition = null)
        {
            foreach (var keyValuePair in objectCache)
                yield return keyValuePair;
        }

        public void Put(string key, object value, string partition = null)
        {
            objectCache.Add(key, value, ObjectCache.InfiniteAbsoluteExpiration, region);
        }

        public void Update(string key, object value, string partition = null)
        {
            objectCache.Set(key, value, region);
        }
    }
}
