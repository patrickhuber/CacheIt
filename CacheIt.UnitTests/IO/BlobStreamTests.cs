using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;
using System.IO;
using System.Text;
using CacheIt.IO;

namespace CacheIt.UnitTests.IO
{
    [TestClass]
    public class BlobStreamTests
    {
        private ObjectCache _cache;
        private string _key;
        private string _region;

        [TestInitialize]
        public void Initialize_BlobStream_Tests()
        {
            _cache = new MemoryCache(Guid.NewGuid().ToString());
            _key = Guid.NewGuid().ToString();
            _region = null;
        }

        [TestMethod]
        public void Test_BlobStream_Read()
        {
            string loremIpsum = LoremIpsum.OneThousandCharacters;
            FillCache(loremIpsum);
            BlobStream blobStream = new BlobStream(_cache, _key, _region);
            byte[] readBytes = new byte[loremIpsum.Length];
            blobStream.Read(readBytes, 0, readBytes.Length);
            Assert.AreEqual(LoremIpsum.OneThousandCharacters, Encoding.ASCII.GetString(readBytes));
        }

        private void FillCache(string text)
        {
            byte[] blob = Encoding.ASCII.GetBytes(LoremIpsum.OneThousandCharacters);
            FillCache(blob);
        }

        private void FillCache(byte[] blob)
        {            
            _cache.Set(_key, blob, _region);
        }

        [TestMethod]
        public void Test_BlobStream_Write()
        {
            string loremIpsum = LoremIpsum.OneThousandCharacters;
            using (BlobStream blobStream = new BlobStream(_cache, _key, _region))
            { 
                byte[] blob = Encoding.ASCII.GetBytes(LoremIpsum.OneThousandCharacters);
                blobStream.Write(blob, 0, blob.Length);
            }
            byte[] actual = _cache.Get(_key, _region) as byte[];
            Assert.IsNotNull(actual);
            Assert.AreEqual(loremIpsum, Encoding.ASCII.GetString(actual));
        }

        [TestMethod]
        public void Test_BlobStream_Save_On_Flush()
        {
            BlobStream blobStream = new BlobStream(_cache, _key, _region);
            byte[] blob = Encoding.ASCII.GetBytes(LoremIpsum.OneThousandCharacters);
            blobStream.Write(blob, 0, blob.Length);
            Assert.IsNull(_cache.Get(_key, _region));
            blobStream.Flush();
            Assert.IsNotNull(_cache.Get(_key, _region));
        }

        [TestMethod]
        public void Test_BlobStream_SetLength_Zero_Removes_Stream()
        {
            string loremIpsum = LoremIpsum.OneThousandCharacters;
            FillCache(loremIpsum);
            
            BlobStream blobStream = new BlobStream(_cache, _key, _region);
            blobStream.SetLength(0);
            Assert.IsNull(_cache.Get(_key, _region));
        }
    }
}
