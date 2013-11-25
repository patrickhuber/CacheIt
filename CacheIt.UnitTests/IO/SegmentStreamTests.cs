using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;
using CacheIt.IO;
using CacheIt;
using System.IO;

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
        public void Test_SetLength_0_Removes_All_Segments()
        {
            var actual = FillCache(1024);
            stream.SetLength(0);
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(0, Key)));
        }

        [TestMethod]
        public void Test_SetLength_BufferSize_Removes_Correct_Segment()
        {
            const int TotalSize = 3072;
            var actual = FillCache(TotalSize);
            stream.SetLength(1024);
            Assert.IsTrue(cache.Contains(SegmentUtility.GenerateSegmentKey(0, Key)));
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(1, Key)));
        }

        [TestMethod]
        public void Test_SetLength_Extend_Adds_Additional_Segments()
        {
            const int TotalSize = 1024;
            var actual = FillCache(TotalSize);
            stream.SetLength(3072);
            for (int i = 0; i < 3;i++)
                Assert.IsTrue(cache.Contains(SegmentUtility.GenerateSegmentKey(i, Key)));
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(3, Key)));
        }

        [TestMethod]
        public void Test_SetLength_Truncate_Removes_Extra_Segments()
        {
            const int TotalSize = 3072;
            var actual = FillCache(TotalSize);
            stream.SetLength(500);
            Assert.IsTrue(cache.Contains(SegmentUtility.GenerateSegmentKey(0, Key)));
            Assert.IsFalse(cache.Contains(SegmentUtility.GenerateSegmentKey(1, Key)));
        }

        #endregion SetLength

        #region Write / Seek / Read

        [TestMethod]
        public void Test_Write_Seek_Read()
        {
            string actual = LoremIpsum.ThreeThousandSixtyNineCharacter;
            byte[] bytes = Encoding.ASCII.GetBytes(actual);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            Assert.AreEqual(Encoding.ASCII.GetString(bytes), actual);
        }

        [TestMethod]
        public void Test_Write_Seek_Read_Seek_Read()
        {
            string testString = LoremIpsum.ThreeThousandSixtyNineCharacter;
            byte[] bytes = Encoding.ASCII.GetBytes(testString);
            byte[] buffer = new byte[1024];
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Seek(263, SeekOrigin.Begin);
            Assert.AreEqual(stream.Read(buffer, 0, 17), 17);
            Assert.AreEqual(testString.Substring(263, 17), Encoding.ASCII.GetString(buffer, 0, 17));
            stream.Seek(172, SeekOrigin.Begin);
            Assert.AreEqual(stream.Read(buffer, 0, 47), 47);
            Assert.AreEqual(testString.Substring(172, 47), Encoding.ASCII.GetString(buffer, 0, 47));
        }
        
        [TestMethod]
        public void Test_Write_Then_Read_Switch_Byte_And_Sequence()
        {
            // setup that found a bug in the lucene library, moving here becasue need to prevent regression
            // in the SegmentStream
            byte[] buffer = new byte[] { 255, 255, 255, 247, 0, 0, 57, 197, 144, 225, 25, 199, 0, 0, 0, 1, 0, 0, 0, 1, 2, 0x5F, 0x30, 0, 0, 0, 1, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 255, 1, 255, 255, 255, 255, 1, 0, 0, 0, 0, 1, 0, 0, 0, 7, 6, 0x73, 0x6F, 0x75, 0x72, 0x63, 0x65, 5, 0x66, 0x6C, 0x75, 0x73, 0x68, 14, 0x6C, 0x75, 0x63, 0x65, 0x6E, 0x65, 0x2E, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 7, 0x33, 0x2E, 0x30, 0x2E, 0x33, 0x2E, 0x30, 2, 0x6F, 0x73, 10, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x5F, 0x4E, 0x54, 7, 0x6F, 0x73, 0x2E, 0x61, 0x72, 0x63, 0x68, 3, 0x78, 0x38, 0x36, 10, 0x6F, 0x73, 0x2E, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 44, 0x4D, 0x69, 0x63, 0x72, 0x6F, 0x73, 0x6F, 0x66, 0x74, 0x20, 0x57, 0x69, 0x6E, 0x64, 0x6F, 0x77, 0x73, 0x20, 0x4E, 0x54, 0x20, 0x36, 0x2E, 0x31, 0x2E, 0x37, 0x36, 0x30, 0x31, 0x20, 0x53, 0x65, 0x72, 0x76, 0x69, 0x63, 0x65, 0x20, 0x50, 0x61, 0x63, 0x6B, 0x20, 0x31, 12, 0x6A, 0x61, 0x76, 0x61, 0x2E, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0, 0x11, 0x6A, 0x61, 0x76, 0x61, 0x2E, 0x76, 0x65, 0x6E, 0x64, 0x6F, 0x72, 0, 0, 0, 0, 0, 0, 0, 0, 0, 252, 58, 236, 128 };
            byte[] overwrite = new byte[] { 0, 0, 0, 0, 252, 58, 236, 129 };
            Array.Copy(overwrite, 0, buffer, 203, overwrite.Length);

            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin);
            for (int i = 0; i < 21; i++)
            {
                var b = stream.ReadByte();
            }
            byte[] read2 = new byte[2];
            Assert.AreEqual(2, stream.Read(read2, 0, read2.Length));
            Assert.AreNotEqual(0, read2[0]);
            Assert.AreNotEqual(0, read2[1]);
        }

        #endregion Write / Seek / Read

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

        private void FillCache(byte[] bytes)
        {
            int size = bytes.Length;
            int startIndex = SegmentUtility.GetSegmentIndex(0, BufferSize);
            int endIndex = SegmentUtility.GetSegmentIndex(size <= 0 ? 0 : size - 1, BufferSize);

            int bytesRead = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                byte[] buffer = new byte[BufferSize];
                int offset = i * BufferSize;
                int count = BufferSize;
                if (offset + count > size)
                    count = size - offset;
                Array.Copy(bytes, bytesRead, buffer, 0, count);
                bytesRead += count;
                cache.Set(SegmentUtility.GenerateSegmentKey(i, Key), buffer);
            }
            cache.Set(Key, new SegmentStreamHeader(BufferSize) { Length = size });
        }
    }
}
