using CacheIt.Index;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.UnitTests.Index
{
    [TestClass]
    public class DistributedIndexInternalNodeTests
    {
        [TestMethod]
        public void Test_DistributedIndexInternalNode_Insert()
        {
            var internalNode = new DistributedIndexInternalNode<int, int>(6);
        }
    }
}
