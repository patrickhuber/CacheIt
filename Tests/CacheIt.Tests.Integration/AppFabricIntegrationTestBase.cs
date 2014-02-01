using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CacheIt.AppFabric.Tests.Integration
{
    public abstract class AppFabricIntegrationTestBase
    {
        public DataCache Cache{get; private set;}
        
        static AppFabricIntegrationTestBase()
        { 
            
        }

        public void Initialize()
        {
            Cache = new DataCacheFactory().GetDefaultCache();
        }
    }
}
