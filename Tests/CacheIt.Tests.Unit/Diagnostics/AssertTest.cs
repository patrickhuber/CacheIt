using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheIt.Tests.Diagnostics
{
    [TestClass]
    public class AssertTest
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Test_Assert_IsNotNull_Throws_Exception()
        {
            CacheIt.Diagnostics.Assert.IsNotNull(null);
            Assert.Fail("Exception expected.");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Test_Assert_IsNotNullOrWhitespace_Throws_Exception_On_WhiteSpace()
        {
            CacheIt.Diagnostics.Assert.IsNotNullOrWhitespace("");
            Assert.Fail("Exception expected");        
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Test_Assert_IsNotNullOrWhitespace_Throws_Exception_On_Null()
        {
            CacheIt.Diagnostics.Assert.IsNotNullOrWhitespace(null);
            Assert.Fail("Exception expected");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Test_Assert_IsNullOrEmpty_Throws_Exception_On_Null()
        {
            int[] array = null;
            CacheIt.Diagnostics.Assert.IsNotNullOrEmpty(array, "");
        }
    }
}
