using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;
using CacheIt.IO;

namespace CacheIt.UnitTests.IO
{
    /// <summary>
    /// Summary description for BufferedStreamTests
    /// </summary>
    [TestClass]
    public class SegmentStreamTests
    {
        private SegmentService segmentService;
        private ObjectCache cache;
        private SegmentStream stream;
        private const string Key = "mykey";
        private const int BufferSize = 1024;

        public SegmentStreamTests()
        {
            segmentService = new SegmentService();
            cache = new MemoryCache("BufferedCacheStreamTests");
            stream = new SegmentStream(cache, Key, segmentService, BufferSize);
        }

        #region  TestContext

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion  TestContext

        [TestMethod]
        public void Test_Write_Large_Array()
        {            
            var bytes = Encoding.ASCII.GetBytes(LoremIpsum.ThreeThousandSixtyNineCharacter);
            stream.Write(bytes, 0, bytes.Length);

            AssertAreEqual(bytes, 0);
        }

        [TestMethod]
        public void Test_Write_Two_Large_Segments()
        {
            string twoThousandCharacters = LoremIpsum.OneThousandCharacters + LoremIpsum.OneThousandCharacters;
            var firstSegment = Encoding.ASCII.GetBytes(twoThousandCharacters);
            var secondSegment = Encoding.ASCII.GetBytes(twoThousandCharacters);
            stream.Write(firstSegment, 0, firstSegment.Length);
            stream.Write(secondSegment, 0, secondSegment.Length);
            AssertAreEqual(firstSegment, 0);
            AssertAreEqual(secondSegment, firstSegment.Length);
        }

        [TestMethod]
        public void Test_Write_Small_Segments_Requires_Flush()
        {
            string[] array = { "what", "does", "the", "fox", "say" };
            foreach (var item in array)
            {
                var buffer = Encoding.ASCII.GetBytes(item);
                stream.Write(buffer, 0, buffer.Length);
                stream.WriteByte((byte)' ');
            }

            byte[] bytes = cache.Get(segmentService.GenerateSegmentKey(0, Key)) as byte[];
            Assert.IsNull(bytes);
        }

        [TestMethod]
        public void Test_Write_Small_Segments_Totaling_Buffer_Size_Auto_Flushes()
        {
            string message = LoremIpsum.OneThousandCharacters + " I only need 24 extra characters this is 43";
            string[] array = message.Split(' ');
            int bytesWritten = 0;
            foreach (var item in array)
            {
                var buffer = Encoding.ASCII.GetBytes(item);
                stream.Write(buffer, 0, buffer.Length);
                stream.WriteByte((byte)' ');
                bytesWritten += buffer.Length + 1;
            }
            byte[] bytes = cache.Get(segmentService.GenerateSegmentKey(0, Key)) as byte[];
            Assert.IsNotNull(bytes);
            bytes = cache.Get(segmentService.GenerateSegmentKey(1, Key)) as byte[];
            Assert.IsNull(bytes);
        }

        private void AssertAreEqual(byte[] expected, int offset)
        {
            for (int index = offset; index < expected.Length; index += BufferSize)
            {
                int segmentIndex = segmentService.GetSegmentIndex(index, BufferSize);
                var buffer = cache.Get(segmentService.GenerateSegmentKey(segmentIndex, Key)) as byte[];
                Assert.IsNotNull(buffer);
                for (int b = 0; b < buffer.Length && index * BufferSize + b < expected.Length; b++)
                {
                    Assert.AreEqual(buffer[b], expected[index * BufferSize + b]);
                }
            }
        }
    }
}
