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
            const int TotalSize = 1024;
            var actual = FillCache(TotalSize);

            byte[] readBuffer = new byte[100];
            stream.Read(readBuffer, 0, readBuffer.Length);

            Assert.AreEqual(actual.Substring(0, 100), Encoding.ASCII.GetString(readBuffer));
        }

        [TestMethod]
        public void Test_Read_Large_Array()
        {
            const int TotalSize = 2048;
            var actual = FillCache(TotalSize);

            byte[] readBuffer = new byte[TotalSize];
            stream.Read(readBuffer, 0, readBuffer.Length);

            Assert.AreEqual(actual, Encoding.ASCII.GetString(readBuffer));
        }

        [TestMethod]
        public void Test_Read_Byte()
        {
            var actual = FillCache(1024);
            var readByte = stream.ReadByte();
            Assert.AreEqual(readByte, actual[0]);
        }

        #endregion Read

        #region Seek

        [TestMethod]
        public void Test_Seek()
        {
            const int TotalSize = 1024;
            var actual = FillCache(TotalSize);
        }
        
        #endregion Seek

        #region Write
        [TestMethod]
        public void Test_Write_Byte()
        {
            byte byteToWrite = Convert.ToByte('a');
            stream.WriteByte(byteToWrite);
            stream.Flush();
            var bytes = cache.Get(SegmentUtility.GenerateSegmentKey(0, Key)) as byte[];
            Assert.AreEqual(byteToWrite, bytes[0]);
        }

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

        #region SetLength
        
        [TestMethod]
        public void SetLength_0_Removes_All_Segments()
        {
            var actual = FillCache(1024);
            stream.SetLength(0);
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(0, Key)));
        }

        [TestMethod]
        public void SetLength_BufferSize_Removes_Correct_Segment()
        {
            const int TotalSize = 3072;
            var actual = FillCache(TotalSize);
            stream.SetLength(1024);
            Assert.IsTrue(cache.Contains(SegmentUtility.GenerateSegmentKey(0, Key)));
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(1, Key)));
        }

        [TestMethod]
        public void SetLength_Extend_Adds_Additional_Segments()
        {
            const int TotalSize = 1024;
            var actual = FillCache(TotalSize);
            stream.SetLength(3072);
            for (int i = 0; i < 3;i++)
                Assert.IsTrue(cache.Contains(SegmentUtility.GenerateSegmentKey(i, Key)));
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(3, Key)));
        }

        [TestMethod]
        public void SetLength_Truncate_Removes_Extra_Segments()
        {
            const int TotalSize = 3072;
            var actual = FillCache(TotalSize);
            stream.SetLength(500);
            Assert.IsTrue(cache.Contains(SegmentUtility.GenerateSegmentKey(0, Key)));
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(1, Key)));
        }

        #endregion SetLength

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

        private string FillCache(int size)
        {
            // allocate the appropriate text blob
            var actual = string.Empty;
            var textLength = LoremIpsum.ThreeThousandSixtyNineCharacter.Length;

            int repeat = size / textLength;
            int remainder = size % textLength;

            for (int i = 0; i < repeat; i++)
                actual += LoremIpsum.ThreeThousandSixtyNineCharacter;
            if (remainder > 0)
                actual += LoremIpsum.ThreeThousandSixtyNineCharacter.Substring(0, remainder);

            // break the blob into buffers
            int startIndex = SegmentUtility.GetSegmentIndex(0, BufferSize);
            int endIndex = SegmentUtility.GetSegmentIndex(size <= 0 ? 0 : size - 1, BufferSize);
                        
            for (int i = startIndex; i <= endIndex; i++)
            {
                byte[] buffer = new byte[BufferSize];
                int offset = i * BufferSize;
                int count = BufferSize;
                if (offset + count > size)
                    count = size - offset;
                byte[] bytes = Encoding.ASCII.GetBytes(actual.Substring(offset, count));
                Array.Copy(bytes, buffer, count);
                cache.Set(SegmentUtility.GenerateSegmentKey(i, Key), buffer);
            }
            cache.Set(Key, new SegmentStreamHeader(BufferSize) { Length = size });
            return actual;
        }
    }
}
