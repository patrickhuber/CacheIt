﻿using System;
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
            
            for(int index = 0;index < bytes.Length;index+=BufferSize)
            {
                int segmentIndex = segmentService.GetSegmentIndex(index, BufferSize);
                var buffer = cache.Get(segmentService.GenerateSegmentKey(segmentIndex, Key));
                Assert.IsNotNull(buffer);
            }
        }

        [TestMethod]
        public void Test_Write_Two_Large_Segments()
        {
            string twoThousandCharacters = LoremIpsum.OneThousandCharacters + LoremIpsum.OneThousandCharacters;
            var firstSegment = Encoding.ASCII.GetBytes(twoThousandCharacters);
            var secondSegment = Encoding.ASCII.GetBytes(twoThousandCharacters);
            stream.Write(firstSegment, 0, firstSegment.Length);
            stream.Write(secondSegment, 0, secondSegment.Length);
        }
    }
}
