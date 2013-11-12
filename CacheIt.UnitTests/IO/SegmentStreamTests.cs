using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;
using CacheIt.IO;
using CacheIt;

namespace CacheIt.UnitTests.IO
{
    /// <summary>
    /// Summary description for BufferedStreamTests
    /// </summary>
    [TestClass]
    public class SegmentStreamTests
    {
        private ObjectCache cache;
        private SegmentStream stream;
        private const string Key = "mykey";
        private const int BufferSize = 1024;

        public SegmentStreamTests()
        {            
            cache = new MemoryCache("BufferedCacheStreamTests");
            stream = new SegmentStream(cache, Key, BufferSize);
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

        #region Read
        
        [TestMethod]
        public void Test_Read_Small_Segment()
        {
            var actual = (LoremIpsum.OneThousandCharacters + LoremIpsum.OneHundredFourtyCharacters).Substring(0, 1024);
            var buffer = Encoding.ASCII.GetBytes(actual);
            cache.Set(SegmentUtility.GenerateSegmentKey(0, Key), buffer);
            cache.Set(Key, new SegmentStreamHeader(1024) { Length = buffer.Length });

            byte[] readBuffer = new byte[100];
            stream.Read(readBuffer, 0, readBuffer.Length);

            Assert.AreEqual(actual.Substring(0, 100), Encoding.ASCII.GetString(readBuffer));
        }

        [TestMethod]
        public void Test_Read_Large_Array()
        {
            const int TotalSize = 2048;

            var actual = LoremIpsum.ThreeThousandSixtyNineCharacter.Substring(0, TotalSize);
            
            // segment 1 |1024|
            var buffer = Encoding.ASCII.GetBytes(actual.Substring(0, BufferSize));            
            cache.Set(SegmentUtility.GenerateSegmentKey(0, Key), buffer);
            
            // segment 2 |1024|
            buffer = Encoding.ASCII.GetBytes(actual.Substring(BufferSize, BufferSize));
            cache.Set(SegmentUtility.GenerateSegmentKey(1, Key), buffer);
            
            // header info
            cache.Set(Key, new SegmentStreamHeader(BufferSize) { Length = buffer.Length });

            byte[] readBuffer = new byte[TotalSize];
            stream.Read(readBuffer, 0, readBuffer.Length);

            Assert.AreEqual(actual, Encoding.ASCII.GetString(readBuffer));
        }

        #endregion Read

        #region Write
        [TestMethod]
        public void Test_Write_Large_Array()
        {            
            var bytes = Encoding.ASCII.GetBytes(LoremIpsum.ThreeThousandSixtyNineCharacter);
            stream.Write(bytes, 0, bytes.Length);

            AssertCacheContentsAreEqualTo(bytes, 0);
        }

        [TestMethod]
        public void Test_Write_Two_Large_Segments()
        {
            string twoThousandCharacters = LoremIpsum.OneThousandCharacters + LoremIpsum.OneThousandCharacters;
            var firstSegment = Encoding.ASCII.GetBytes(twoThousandCharacters);
            var secondSegment = Encoding.ASCII.GetBytes(twoThousandCharacters);
            stream.Write(firstSegment, 0, firstSegment.Length);
            stream.Write(secondSegment, 0, secondSegment.Length);
            AssertCacheContentsAreEqualTo(firstSegment, 0);
            AssertCacheContentsAreEqualTo(secondSegment, firstSegment.Length);
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

            byte[] bytes = cache.Get(SegmentUtility.GenerateSegmentKey(0, Key)) as byte[];
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
            byte[] bytes = cache.Get(SegmentUtility.GenerateSegmentKey(0, Key)) as byte[];
            Assert.IsNotNull(bytes);
            bytes = cache.Get(SegmentUtility.GenerateSegmentKey(1, Key)) as byte[];
            Assert.IsNull(bytes);
        }

        #endregion Write

        #region Length
        [TestMethod]
        public void Test_Length_Updates_After_Write_And_Flush()
        {
            byte[] buffer = Encoding.ASCII.GetBytes(new string('a', 1024));
            stream.Write(buffer, 0, buffer.Length);
            Assert.AreEqual((long)buffer.Length, stream.Length);
        }

        [TestMethod]
        public void Test_Length_Updates_After_Small_Write()
        {
            byte[] buffer = Encoding.ASCII.GetBytes(new string('a', 10));
            stream.Write(buffer, 0, buffer.Length);
            Assert.AreEqual((long)buffer.Length, stream.Length);
        }
        #endregion Length

        private void AssertCacheContentsAreEqualTo(byte[] expected, int offset)
        {
            for (int index = offset; index < expected.Length; index += BufferSize)
            {
                int segmentIndex = SegmentUtility.GetSegmentIndex(index, BufferSize);
                var buffer = cache.Get(SegmentUtility.GenerateSegmentKey(segmentIndex, Key)) as byte[];
                Assert.IsNotNull(buffer);
                for (int b = 0; b < buffer.Length && index * BufferSize + b < expected.Length; b++)
                {
                    Assert.AreEqual(buffer[b], expected[index * BufferSize + b]);
                }
            }
        }
    }
}
