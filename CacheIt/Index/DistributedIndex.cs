using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.Index
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TPointer">The type of the pointer.</typeparam>
    public class DistributedIndex<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        private ObjectCache _cache;
        private readonly int _order;
        private RegionKey _regionKey;
        private DistributedIndexLeafNode<TKey, TPointer> _root;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedIndex{TKey, TPointer}"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="order">The order.</param>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public DistributedIndex(ObjectCache cache, int order, string key, string region = null)
        {
            _cache = cache;
            _order = order;
            _regionKey = new RegionKey { Key = key, Region = region };
            _root = _cache.Get<DistributedIndexLeafNode<TKey, TPointer>>(
                _regionKey.Key, 
                () => new DistributedIndexLeafNode<TKey, TPointer>(_order), 
                _regionKey.Region);
        }

        /// <summary>
        /// Inserts the specified key in the index storing the address in the root.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="pointer">The address.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Insert(TKey key, TPointer pointer)
        {
            InsertRecursive(key, pointer, _root);

            // check for full root. if full, split
            if (_root.IsFull())
            {
                // root is a leaf node, 
                // split to other leaf
                // create internal node as new root
                if (_root.IsLeaf)
                { 
                }
                // root is internal node.
                // split to other internal
                // create internal as new root
                else
                {
                }
            }
        }

        private void InsertRecursive(TKey key, TPointer pointer, DistributedIndexNode<TKey, TPointer> node)
        {
            if (node.IsLeaf)
            {
                node.Insert(key, pointer);
                return;
            }
            
            // find the index
            var index = node.FindIndex(key);
            
            // if the key is greater than the largest key, we have a new largest key            
            if (key.CompareTo(node.GetLargestKey()) > 0)
                index += 1;

            var internalNode = node as DistributedIndexInternalNode<TKey, TPointer>;
            var childAddress = internalNode.GetChildAddress(index);
            var childNode = Load(childAddress);

            InsertRecursive(key, pointer, childNode);
        }

        public void Remove(TKey key)
        { 

        }

        private DistributedIndexNode<TKey, TPointer> Load(string pointer)
        {
            return _cache.Get(pointer, _regionKey.Region) as DistributedIndexNode<TKey, TPointer>;
        }

        private void Store(DistributedIndexNode<TKey, TPointer> node)
        {
            _cache.Set(node.Address, node, node.Region);
        }

        /// <summary>
        /// Searches the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public IEnumerable<TPointer> Search(TKey key)
        {
            var leafNode = FindLeaf(key);
            return leafNode.Search(key);
        }

        private DistributedIndexLeafNode<TKey, TPointer> FindLeaf(TKey key)
        {
            throw new NotImplementedException();
        }
    }
}
