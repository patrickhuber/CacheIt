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
    public class BufferedStreamTests
    {
        private ObjectCache cache;
        private BufferedStream stream;
        private const string Key = "mykey";
        public BufferedStreamTests()
        {
            cache = new MemoryCache("BufferedStreamTests");
            stream = new BufferedStream(cache, Key, null, 1024);
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
        public void Test_Write()
        {
            long length = LoremIpsum.OneThousandCharacters.Length;
        }
    }
}
