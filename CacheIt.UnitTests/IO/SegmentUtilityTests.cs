using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CacheIt.IO;

namespace CacheIt.UnitTests.IO
{
    /// <summary>
    /// Summary description for SegmentServiceTests
    /// </summary>
    [TestClass]
    public class SegmentUtilityTests
    {
        public SegmentUtilityTests()
        {
        }

        #region TestContext
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

        #endregion TestContext

        [TestMethod]
        public void Test_SegmentUtility_GenerateSegmentKey()
        {
            string actual = SegmentUtility.GenerateSegmentKey(0, "mykey");
            Assert.AreEqual("mykey_0", actual);
        }

        [TestMethod]
        public void Test_SegmentUtility_GetPositionInSegment()
        {
            int actual = SegmentUtility.GetPositionInSegment(1024, 1024);
            Assert.AreEqual(0, actual);
            actual = SegmentUtility.GetPositionInSegment(1025, 1024);
            Assert.AreEqual(1, actual);
            actual = SegmentUtility.GetPositionInSegment(1023, 1024);
            Assert.AreEqual(1023, actual);
            actual = SegmentUtility.GetPositionInSegment(2048, 1024);
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void Test_SegmentUtility_GetSegmentIndex()
        {
            int actual = SegmentUtility.GetSegmentIndex(0, 1024);
            Assert.AreEqual(0, actual);
            actual = SegmentUtility.GetSegmentIndex(1025, 1024);
            Assert.AreEqual(1, actual);
        }
    }
}
