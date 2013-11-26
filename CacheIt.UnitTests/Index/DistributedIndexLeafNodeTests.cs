using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CacheIt.Index;
using System.Collections.Generic;

namespace CacheIt.UnitTests.Index
{
    [TestClass]
    public class DistributedIndexLeafNodeTests
    {
        [TestMethod]
        public void Test_DistributedIndexLeafNode_Insert()
        {
            var dictionary = new Dictionary<int, int>()
                { {10, 1}, {20,2}, {30, 3}, {40, 4}, {35, 5}, {5, 6}};
            var node = new DistributedIndexLeafNode<int, int>(6);
            foreach(var key in dictionary.Keys)
            {
                node.Insert(key, dictionary[key]);
            }
            Assert.IsTrue(node.IsFull());
            int i = 0;
            foreach(var key in dictionary.Keys)
            {                
                var pointerList = node.GetPointerListByKey(key);
                Assert.AreEqual(1, pointerList.Count());
                Assert.AreEqual(dictionary[key], pointerList.First());
                i++;
            }     
        }

        [TestMethod]
        public void Test_DistributedIndexLeafNode_Split()
        {
            var dictionary = new Dictionary<int, int>() { { 10, 1 }, { 20, 2 }, { 30, 3 }, { 40, 4 }, { 35, 5 }, { 5, 6 } };
            var node = new DistributedIndexLeafNode<int, int>(6);
            foreach (var key in dictionary.Keys)
            {
                node.Insert(key, dictionary[key]);
            }
            var overflowNode = new DistributedIndexLeafNode<int, int>(6);
            Assert.IsTrue(node.Split(overflowNode));
            Assert.AreEqual(3, node.GetKeys().Count());
            Assert.AreEqual(3, overflowNode.GetKeys().Count());
        }

        [TestMethod]
        public void Test_DistributedIndexLeafNode_Split_Not_Full()
        {
            var dictionary = new Dictionary<int, int>() { { 10, 1 }, { 20, 2 }, { 30, 3 }, { 40, 4 }, { 35, 5 } };
            var node = new DistributedIndexLeafNode<int, int>(6);
            foreach (var key in dictionary.Keys)
            {
                node.Insert(key, dictionary[key]);
            }
            var overflowNode = new DistributedIndexLeafNode<int, int>(6);
            Assert.IsFalse(node.Split(overflowNode));
            Assert.AreEqual(5, node.GetKeys().Count());
        }

        [TestMethod]
        public void Test_DistributedIndexLeafNode_Merge()
        {
            var dictionary = new Dictionary<int, int>() { { 10, 1 }, { 20, 2 }, { 30, 3 }, { 40, 4 }, { 35, 5 }};
            var node = new DistributedIndexLeafNode<int, int>(6);
            var fromNode = new DistributedIndexLeafNode<int, int>(6);
            int midpoint = (dictionary.Keys.Count + 1) / 2;
            int i = 0;
            foreach (var key in dictionary.Keys.OrderBy(x=>x))
            {
                if (i < midpoint)
                    node.Insert(key, dictionary[key]);
                else
                    fromNode.Insert(key, dictionary[key]);
                i++;
            }
            Assert.IsTrue(node.Merge(fromNode));
        }
    }
}
