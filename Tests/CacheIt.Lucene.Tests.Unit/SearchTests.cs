using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lucene.Net.Store;

namespace CacheIt.Lucene.Tests.Unit
{
    [TestClass]
    public class SearchTests
    {
        SimpleDataRepository repository;

        [TestInitialize]
        public void Initialize_SearchTests()
        {
            repository = new SimpleDataRepository();
        }

        [TestMethod]
        public void Test_Search_USA_Returns_One_Result()
        {
            var results = repository.Search("USA");
            Assert.AreEqual(1, results.Count());
        }
    }
}
