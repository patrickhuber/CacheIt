using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CacheIt.Index;
using System.Runtime.Caching;

namespace CacheIt.UnitTests.Index
{
    [TestClass]
    public class IndexTreeTests
    {
        private ObjectCache _cache;

        [TestInitialize]
        public void Initialize_IndexTree_Tests()
        {
            _cache = new MemoryCache(Guid.NewGuid().ToString());
        }

        [TestMethod]
        public void Test_IndexTree_Insert()
        {
            IndexTree<int, int> indexTree = new IndexTree<int, int>(_cache, 3, Guid.NewGuid().ToString());
            for (int i = 1; i < 10; i++)
                indexTree.Insert(i, i);
        }
    }
}
