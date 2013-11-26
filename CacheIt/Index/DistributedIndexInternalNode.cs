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
        }

        public DistributedIndexInternalNode(int order)
            : base(order)
        { 
        
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
