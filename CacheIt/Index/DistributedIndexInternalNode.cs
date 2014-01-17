using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Index
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TPointer">The type of the pointer.</typeparam>
    public sealed class DistributedIndexInternalNode<TKey, TPointer> : DistributedIndexNode<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        private IList<string> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedIndexInternalNode{TKey, TPointer}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <param name="cacheKey">The cache key.</param>
        /// <param name="cacheRegion">The cache region.</param>
        public DistributedIndexInternalNode(int order, string cacheKey, string cacheRegion = null)
            : base(order, cacheKey, cacheRegion)
        {
            _children = new List<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DistributedIndexInternalNode{TKey, TPointer}"/> class.
        /// </summary>
        /// <param name="order">The order.</param>
        public DistributedIndexInternalNode(int order)
            : base(order)
        {
            _children = new List<string>();
        }

        /// <summary>
        /// Gets the child address.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public string GetChildAddress(int index)
        {
            if (index >= _children.Count || index < 0)
                throw new ArgumentException(
                    string.Format("Invalid index {0}. Index must be between 0 and {1}", index, _children.Count));
            return _children[index];
        }

        /// <summary>
        /// Gets a value indicating whether [is leaf].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is leaf]; otherwise, <c>false</c>.
        /// </value>
        public override bool IsLeaf
        {
            get { return false; }
        }

        protected override void OnInsertIndexFound(TKey key, TPointer pointer, int index)
        {
            throw new NotImplementedException();
        }

        protected override void OnSplitKeyMoved(DistributedIndexNode<TKey, TPointer> overflow, int index)
        {
            if (!overflow.IsLeaf)
            {
                var internalNode = overflow as DistributedIndexInternalNode<TKey, TPointer>;
            }
        }

        /// <summary>
        /// Merges the specified other.
        /// </summary>
        /// <param name="other">The other.</param>
        /// <returns></returns>
        public override bool Merge(DistributedIndexNode<TKey, TPointer> other)
        {
            var result = base.Merge(other);
            if (!IsLeaf)
            {
                var internalNode = other as DistributedIndexInternalNode<TKey, TPointer>;
                
                // after the movement we need to grab the last child aka, the 'infinity' pointer
                if (internalNode._children.Count > 0)
                {
                    _children.Add(internalNode._children[0]);
                    internalNode._children.RemoveAt(0);
                }
            }
            return result;
        }

        protected override void OnMergeKeyMoved(DistributedIndexNode<TKey, TPointer> other, int index)
        {
            if (!other.IsLeaf)
            {
                var internalNode = other as DistributedIndexInternalNode<TKey, TPointer>;
                _children.Add(internalNode._children[index]);
                internalNode._children.RemoveAt(index);
            }
        }
    }
}
