using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ApplicationServer.Caching;
using CacheIt.AppFabric;
using CacheIt.IO;
using System.IO;
using System.Diagnostics;

namespace CacheIt.AppFabric.Tests.Integration.Learning
{
    [TestClass]
    public class AppFabricTests : AppFabricUnitTestBase
    {
        [TestInitialize]
        public void AppFabricTests_Initialize()
        {
            Initialize();
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
            Cache.Put("testInstance", testInstance);
            testInstance.Name = "not my instance";
            testInstance = Cache.Get("testInstance") as TestClass;
            Assert.AreEqual("my instance", testInstance.Name);
            Cache.Remove("testInstance");
        }

        [TestMethod]
        public void Test_AppFabric_Add_Througput()
        {
            Stopwatch stopWatch = Stopwatch.StartNew();            
            for (int i = 0; i < 1000; i++)
            {
                var testInstance = new TestClass();
                testInstance.Name = CreateKey(i);
                Cache.Put(testInstance.Name, testInstance);
            }
            stopWatch.Stop();
            Assert.IsTrue(1000 > stopWatch.ElapsedMilliseconds);
            for (int i = 0; i < 10000; i++)
            {
                string key = CreateKey(i);
                Cache.Remove(key);
            }
        }

        private string CreateKey(int i)
        {
            return string.Format("instance {0}", i);
        }
    }
}
