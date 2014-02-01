using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;

namespace CacheIt.AppFabric.Tests.Integration
{
    [TestClass]
    public class AppFabricCacheTests : AppFabricIntegrationTestBase
    {
        private ObjectCache objectCache;

        [TestInitialize]
        public void Initialize_AppFabricCache_Tests()
        {
            base.Initialize();
            objectCache = new AppFabricCache(Cache);
        }

        [TestMethod]
        public void Test_AppFabricCache_Add_Returns_True()
        {
            var result = objectCache.Add("test", "mystring", ObjectCache.InfiniteAbsoluteExpiration);
            Assert.IsTrue(result);
        }        
    }
}
