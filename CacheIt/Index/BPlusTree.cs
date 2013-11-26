using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.Index
{
    public class BPlusTree<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        private BPlusInternalNode<TKey, TPointer> _root;

        public BPlusTree(ObjectCache cache, int degree, string key, string region = null)
        {
            _root = new BPlusInternalNode<TKey, TPointer>(cache, degree, key, region);
        }

        public BPlusNode<TKey, TPointer> Search(TKey key)
        {
            return Search(key, _root);
        }

        private BPlusNode<TKey, TPointer> Search(TKey key, BPlusNode<TKey, TPointer> node)
        {
            if (node.NodeType == BPlusNodeType.Leaf)
                return node;
            var internalNode = node as BPlusInternalNode<TKey, TPointer>;
            int i = 0;
            foreach(var nodeKey in internalNode.Keys)
            {
                if (key.CompareTo(nodeKey) <= 0)
                {
                    return Search(key, internalNode.GetChildAt(i));
                }
                i++;
            }
            return Search(key, internalNode.GetChildAt(i + 1));
        }

        public void Insert(TKey key, TPointer pointer)
        {
            var node = Search(key);

        }

        public void Delete(TKey key)
        { }
        public void Delete(TKey key, TPointer pointer)
        { }
    }
}
