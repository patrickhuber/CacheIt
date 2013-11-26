using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Index
{
    /// <summary>
    /// Provides an abstract base class for distributed index node operations
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TPointer">The type of the pointer.</typeparam>
    [Serializable]
    public abstract class DistributedIndexNode<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        protected RegionKey regionKey;
        protected IList<TKey> keys;
        protected readonly int order;
        protected readonly int minimumKeyCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedIndexNode{TKey, TPointer}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheRegion">The cache region.</param>
        /// <exception cref="System.ArgumentException">order must be greater than 2</exception>
        protected DistributedIndexNode(int order, string cacheKey, string cacheRegion = null)
        {
            if (order < 2)
                throw new ArgumentException("order must be greater than 2");

            this.keys = new List<TKey>();
            this.order = order;
            this.minimumKeyCount = (order - 1) / 2;
            this.regionKey = new RegionKey { Key = cacheKey, Region = cacheRegion };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedIndexNode{TKey, TPointer}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        protected DistributedIndexNode(int order)
            : this(order, Guid.NewGuid().ToString())
        { }

        /// <summary>
        /// Gets a value indicating whether the node is a leaf
        /// </summary>
        /// <value>
        ///   <c>true</c> if the node is a leaf; otherwise, <c>false</c>.
        /// </value>
        public abstract bool IsLeaf { get; }
        
        /// <summary>
        /// Gets the cache key.
        /// </summary>
        /// <value>
        /// The cache key.
        /// </value>
        public string Address { get { return regionKey.Key; } }

        /// <summary>
        /// Gets the cache region.
        /// </summary>
        /// <value>
        /// The cache region.
        /// </value>
        public string Region { get { return regionKey.Region; } }

        /// <summary>
        /// returns the largest key
        /// </summary>
        /// <returns></returns>
        public TKey GetLargestKey()
        {
            return keys.Max();
        }

        /// <summary>
        /// Gets a value indicating whether the node is full.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is full]; otherwise, <c>false</c>.
        /// </value>
        public bool IsFull()
        {
            return keys.Count == order;
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="pointer">The pointer.</param>
        /// <returns></returns>
        public virtual int Insert(TKey key, TPointer pointer)
        {
            // check for the key in the list of keys
            var index = keys.IndexOf(key);

            // if the key doesn't exist
            if (index < 0)
            {
                // perform an ordered insert
                index = keys.Count;
                for (int i = 0; i < keys.Count; i++)
                {
                    if (key.CompareTo(keys[i]) < 0)
                    {
                        index = i;
                        break;
                    }
                }
                keys.Insert(index, key);
                OnInsertIndexFound(key, pointer, index);
            }
            return index;
        }

        /// <summary>
        /// Called when the Insert Index Is Found
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pointer"></param>
        /// <param name="index"></param>
        protected abstract void OnInsertIndexFound(TKey key, TPointer pointer, int index);

        /// <summary>
        /// Splits the specified overflow.
        /// </summary>
        /// <param name="overflow">The overflow.</param>
        /// <returns></returns>
        public virtual bool Split(DistributedIndexNode<TKey, TPointer> overflow)
        {
            if (!IsFull())
            {
                // throw new InvalidOperationException("Split is only to be called on nodes that are full.");
                return false;
            }

            // find the first key to be moved into the new node
            int midpoint = (keys.Count + 1) / 2;
            int newKeyCount = keys.Count - midpoint;

            // check that the number of keys for the new node is OK
            if (newKeyCount > order || newKeyCount < minimumKeyCount)
            {
                return false;
                // throw new InvalidOperationException("Split does not have the appropriate key counts to complete the operation.");
            }

            while (midpoint != keys.Count)
            {
                overflow.keys.Add(keys[midpoint]);
                keys.RemoveAt(midpoint);

                OnSplitKeyMoved(overflow, midpoint);                
            }
            return true;
        }

        /// <summary>
        /// Called from the split method when a key is moved.
        /// </summary>
        /// <param name="overflow">The overflow node.</param>
        /// <param name="index">The index being processed.</param>
        protected abstract void OnSplitKeyMoved(DistributedIndexNode<TKey, TPointer> overflow, int index);

        /// <summary>
        /// Merges the current node with the specified other node.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public virtual bool Merge(DistributedIndexNode<TKey, TPointer> other)
        {
            // check for too many keys
            if (keys.Count + other.keys.Count > order - 1)
                return false;

            while (other.keys.Count > 0)
            {
                keys.Add(other.keys[0]);
                other.keys.RemoveAt(0);
                OnMergeKeyMoved(other, 0);
            }

            return true;
        }

        /// <summary>
        /// Called when a merge key is moved.
        /// </summary>
        /// <param name="other">The other node..</param>
        /// <param name="index">The index.</param>
        protected abstract void OnMergeKeyMoved(DistributedIndexNode<TKey, TPointer> other, int index);
        
        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TKey> GetKeys()
        {
            return keys;
        }
    }
}
