using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace CacheIt.AppFabric.Tests.Integration
{
    public abstract class AppFabricUnitTestBase
    {
        public DataCache Cache{get; private set;}
        
        static AppFabricUnitTestBase()
        { 
            
        }

        public void Initialize()
        {
            Cache = new DataCacheFactory().GetDefaultCache();
        }
    }
}
