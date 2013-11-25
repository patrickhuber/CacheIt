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
    }
}
