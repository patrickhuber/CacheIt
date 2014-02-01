using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CacheIt.IO;

namespace CacheIt.AppFabric.Tests.Integration.IO
{
    [TestClass]
    public class SegmentStreamTests : AppFabricUnitTestBase
    {
        [TestInitialize]
        public void AppFabricTests_Initialize()
        {
            Initialize();
        }

        [TestMethod]
        public void Test_SegmentStream_Stream_Write_To_AppFabric()
        {
            const string Key = "hello-world.txt";

            string testFile = Key;
            string expectedFileContents =
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

            AppFabricCache appFabricCache = new AppFabricCache(Cache);

            // create a chunk stream and write the file contents using the chunk stream
            SegmentStream segmentStream = new SegmentStream(appFabricCache, testFile);
            using (StreamWriter streamWriter = new StreamWriter(segmentStream))
            {
                streamWriter.Write(expectedFileContents);
            }

            // the chunk stream should be disposed (because it implements IDisposable)
            segmentStream = new SegmentStream(appFabricCache, testFile);

            // create a variable to hold the contents of the current stream
            string actualFileContents = string.Empty;
            using (StreamReader streamReader = new StreamReader(segmentStream))
            {
                actualFileContents = streamReader.ReadToEnd();
            }

            // asser the the expected file contents match the actual file contents
            Assert.AreEqual(expectedFileContents, actualFileContents);

            segmentStream = new SegmentStream(appFabricCache, Key);
            segmentStream.SetLength(0);
            appFabricCache.Remove(Key);
        }
    }
}
