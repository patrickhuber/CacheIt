using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    public class DataCacheFactoryAdapter : IDataCacheFactory
    {
        private DataCacheFactory dataCacheFactory;

        public DataCacheFactoryAdapter(DataCacheFactory dataCacheFactory)
        {
            this.dataCacheFactory = dataCacheFactory;
        }

        public IDataCache GetCache(string cacheName)
        {
            return new DataCacheAdapter(dataCacheFactory.GetCache(cacheName));
        }
    }
}
