using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Index
{
    /// <summary>
    /// Represents a node in the index
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TPointer">The type of the pointer.</typeparam>
    public sealed class DistributedIndexLeafNode<TKey, TPointer> : DistributedIndexNode<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        private IList<IList<TPointer>> _pointers;

        /// <summary>
        /// Gets the next.
        /// </summary>
        /// <value>
        /// The next.
        /// </value>
        public string Next { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedIndexLeafNode{TKey, TPointer}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public DistributedIndexLeafNode(int order)
            : base(order)
        {
            _pointers = new List<IList<TPointer>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedIndexLeafNode{TKey, TPointer}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <exception cref="System.ArgumentException">order must be greater than 2</exception>
        public DistributedIndexLeafNode(int order, string cacheKey, string cacheRegion = null)
            : base(order, cacheKey, cacheRegion)
        {            
            _pointers = new List<IList<TPointer>>();
        }

        /// <summary>
        /// Searches the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<TPointer> Search(TKey key)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Determines whether this instance is leaf.
        /// </summary>
        /// <returns></returns>
        public override bool IsLeaf
        {
            get{return true;}
        }

        /// <summary>
        /// Inserts the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="address">The address.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int Insert(TKey key, TPointer address)
        {
            int index = base.Insert(key, address);

            // we now have the key in the key list.
            // get the list of pointers
            var pointerList = _pointers[index];

            // if the pointer doesn't exist, add it to the pointer list
            // TODO: make this a distributed list insert ot help with large pointer counts
            if (!pointerList.Contains(address))
                pointerList.Add(address);
            
            return index;
        }

        /// <summary>
        /// Called when [insert index found].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="pointer">The pointer.</param>
        /// <param name="index">The index.</param>
        protected override void OnInsertIndexFound(TKey key, TPointer pointer, int index)
        {
            _pointers.Insert(index, new List<TPointer>());            
        }


        /// <summary>
        /// Splits the current node distributing to the overflow node.
        /// </summary>
        /// <param name="overflow">The overflow.</param>
        /// <returns></returns>
        public override bool Split(DistributedIndexNode<TKey, TPointer> overflow)
        {
            var result =  base.Split(overflow);
            if (overflow.IsLeaf)
            {
                var leaf = overflow as DistributedIndexLeafNode<TKey, TPointer>;
                leaf.Next = this.Next;
                this.Next = leaf.Address;
            }
            return result;
        }

        /// <summary>
        /// Called when [split key moved].
        /// </summary>
        /// <param name="overflow">The overflow.</param>
        /// <param name="index">The index.</param>
        protected override void OnSplitKeyMoved(DistributedIndexNode<TKey, TPointer> overflow, int index)
        {
            if (overflow.IsLeaf)
            {
                var leaf = overflow as DistributedIndexLeafNode<TKey, TPointer>;
                leaf._pointers.Add(_pointers[index]);
                _pointers.RemoveAt(index);                
            }
        }

        /// <summary>
        /// Merges the specified node.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public override bool Merge(DistributedIndexNode<TKey, TPointer> other)
        {            
            var result = base.Merge(other);
            if (other.IsLeaf)
            {
                var leaf = other as DistributedIndexLeafNode<TKey, TPointer>;
                this.Next = leaf.Next;
            }
            return result;
        }

        /// <summary>
        /// Called when [merge key moved].
        /// </summary>
        /// <param name="other">The other.</param>
        /// <param name="index">The index.</param>
        protected override void OnMergeKeyMoved(DistributedIndexNode<TKey, TPointer> other, int index)
        {
            if (other.IsLeaf)
            {
                var leaf = other as DistributedIndexLeafNode<TKey, TPointer>;
                _pointers.Add(leaf._pointers[0]);
                leaf._pointers.RemoveAt(0);
            }
        }

        /// <summary>
        /// Gets the pointer list by key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public IEnumerable<TPointer> GetPointerListByKey(TKey key)
        {
            var index = keys.IndexOf(key);
            return GetPointerList(index);
        }

        /// <summary>
        /// Gets the pointer list.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public IEnumerable<TPointer> GetPointerList(int index)
        {
            if (index > _pointers.Count - 1 || index < 0)
                throw new IndexOutOfRangeException(
                    string.Format("index {0} is out of range 0 to {1}", index, _pointers.Count));
            return _pointers[index];
        }
    }
}
