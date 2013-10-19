using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ApplicationServer.Caching;

namespace CacheIt.IntegrationTests
{
    [TestClass]
    public class AppFabricTests
    {
        DataCache cache;

        [TestInitialize]
        public void AppFabricTests_Initialize()
        {
            cache = new DataCacheFactory().GetDefaultCache();
        }
        
        [Serializable]
        private class TestClass
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public void Test_Same_Object_In_Cache()
        {            
            var testInstance = new TestClass();
            testInstance.Name = "my instance";
            cache.Put("testInstance", testInstance);
            testInstance.Name = "not my instance";
            testInstance = cache.Get("testInstance") as TestClass;
            Assert.AreEqual("my instance", testInstance.Name);
            cache.Remove("testInstance");
        }
    }
}
