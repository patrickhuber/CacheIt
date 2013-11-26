using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CacheIt.Index;
using System.Runtime.Caching;

namespace CacheIt.UnitTests.Index
{
    [TestClass]
    public class DistributedIndexTests
    {
        private DistributedIndex<string, string> index;
        private string key;
        private ObjectCache cache;

        [TestInitialize]
        public void Initialize_DistributedIndex_Tests()
        {
            cache = new MemoryCache(Guid.NewGuid().ToString());
            key = Guid.NewGuid().ToString();
            index = new DistributedIndex<string, string>(cache, 6, key);
        }

        [TestMethod]
        public void Test_DistributedIndex_Search()
        {
            foreach (var country in Country.Countries)
            { 
                index.Insert(country.Code, country.Name);
            }
            var result = index.Search("US");
        }
    }
}
