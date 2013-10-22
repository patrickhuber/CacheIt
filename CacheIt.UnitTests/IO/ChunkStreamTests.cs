using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Caching;
using CacheIt.IO;

namespace CacheIt.UnitTests.IO
{
    [TestClass]
    public class ChunkStreamTests
    {
        private ObjectCache objectCache;
        private ChunkStream stream;
        private const string Key = "myfile.txt";
        private const string FirstRecordKey = Key + "_0";
        private const string SecondRecordKey = Key + "_1";
        private const string ThirdRecordKey = Key + "_2";

        [TestInitialize]
        public void Initialize_AppFabricStreamTest()
        {
            objectCache = new MemoryCache("test");
            stream = new CacheIt.IO.ChunkStream(objectCache, Key);
        }

        [TestMethod]
        public void Test_Constructor_Creates_Header()
        {
            Assert.IsNotNull(stream.Header);
            Assert.IsTrue(objectCache.Contains(Key));
        }

        #region Read

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Read_Null_Buffer_Throws_ArgumentNullException()
        {
            stream.Read(null, 0, 1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Read_Small_Buffer_Length_Throws_ArgumentException()
        {
            byte[] buffer = new byte[10];
            stream.Read(buffer, 0, 15);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Read_Negative_Count_Throws_ArgumentOutOfRangeException()
        {
            byte[] buffer = new byte[10];
            stream.Read(buffer, 0, -5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Read_Negative_Offset_Throws_ArgumentOutOfRangeException()
        {
            byte[] buffer = new byte[10];
            stream.Read(buffer, -5, 5);
        }

        [TestMethod]
        public void Test_Read_Returns_Zero_For_Empty_Stream()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            Assert.AreEqual(0, bytesRead);
        }

        #endregion Read

        #region Seek

        [TestMethod]
        public void Test_Seek_Creates_Advances_Position()
        {
            var absoluteOffset = stream.Seek(1024, System.IO.SeekOrigin.Begin);

            Assert.AreEqual(1024, absoluteOffset);
            Assert.AreEqual(absoluteOffset, stream.Position);
            Assert.AreEqual(absoluteOffset, stream.Header.Length);
            Assert.IsTrue(objectCache.Contains(Key));
            Assert.IsFalse(objectCache.Contains(FirstRecordKey));
        }

        [TestMethod]
        public void Test_Seek_Creates_New_Records()
        {
            var absoluteOffset = stream.Seek(1025, SeekOrigin.Begin);

            Assert.AreEqual(1025, absoluteOffset);
            Assert.AreEqual(absoluteOffset, stream.Position);
            Assert.IsTrue(objectCache.Contains(Key));
            Assert.IsTrue(objectCache.Contains(FirstRecordKey));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Seek_Negative_Seek_From_Begin_Throws_ArgumentOutOfRangeException()
        {
            stream.Seek(-10, System.IO.SeekOrigin.Begin);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Seek_Relative_Current_Negative_Seek_Throws_ArgumentOutOfRangeException()
        {
            stream.Seek(-10, System.IO.SeekOrigin.Current);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Seek_Relative_End_Negative_Seek_Throws_ArgumentOutOfRangeException()
        {
            stream.Seek(-10, System.IO.SeekOrigin.End);
        }

        #endregion Seek

        #region Flush

        [TestMethod]
        public void Test_Flush_Creates_New_Record()
        {
            stream.Flush();
            Assert.AreEqual(0, stream.Position);
            Assert.IsTrue(objectCache.Contains(Key));
            Assert.IsTrue(objectCache.Contains(FirstRecordKey));
        }

        #endregion Flush

        #region Write

        [TestMethod]
        public void Test_Write_Creates_New_Record()
        {
            byte[] textBytes = ASCIIEncoding.ASCII.GetBytes(LoremIpsum.OneThousandCharacters + LoremIpsum.OneHundredFourtyCharacters);
            byte[] bytes = new byte[2000];
            Array.Copy(textBytes, bytes, 2000);

            stream.Write(bytes, 0, bytes.Length);

            Assert.IsTrue(objectCache.Contains(Key));
            Assert.IsTrue(objectCache.Contains(FirstRecordKey));
            Assert.IsTrue(objectCache.Contains(SecondRecordKey));
            Assert.IsFalse(objectCache.Contains(ThirdRecordKey));
            Assert.AreEqual(stream.Position, stream.Header.Length);
        }

        #endregion Write
    }
}
