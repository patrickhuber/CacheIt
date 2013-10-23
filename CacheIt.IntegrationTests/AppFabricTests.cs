using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.ApplicationServer.Caching;
using CacheIt.AppFabric;
using CacheIt.IO;
using System.IO;

namespace CacheIt.IntegrationTests
{
    [TestClass]
    public class AppFabricTests
    {
        // create the object cache instance from the object cache        
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

        [TestMethod]
        public void Test_Stream_Write_To_AppFabric()
        {            
            const string Key = "hello-world.txt";

            string testFile = Key;
            string expectedFileContents =
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";
            
            AppFabricCache appFabricCache = new AppFabricCache(cache);

            // create a chunk stream and write the file contents using the chunk stream
            ChunkStream chunkStream = new ChunkStream(appFabricCache, testFile);
            using (StreamWriter streamWriter = new StreamWriter(chunkStream))
            {
                streamWriter.Write(expectedFileContents);
            }

            // the chunk stream should be disposed (because it implements IDisposable)
            chunkStream = new ChunkStream(appFabricCache, testFile);

            // create a variable to hold the contents of the current stream
            string actualFileContents = string.Empty;
            using (StreamReader streamReader = new StreamReader(chunkStream))
            {
                actualFileContents = streamReader.ReadToEnd();
            }

            // asser the the expected file contents match the actual file contents
            Assert.AreEqual(expectedFileContents, actualFileContents);

            chunkStream = new ChunkStream(appFabricCache, Key);
            chunkStream.SetLength(0);
            appFabricCache.Remove(Key);
        }
    }
}
