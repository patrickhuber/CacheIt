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
    /// <typeparam name="TKey">The type of the key</typeparam>
    /// <typeparam name="TPointer">The type of the unique identifier.</typeparam>
    public class BPlusInternalNode<TKey, TPointer> : BPlusNode<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        private IList<string> _childNodes;

        public BPlusInternalNode(ObjectCache objectCache, int degree, string key, string region = null)
            : base(objectCache, degree, key, region)
        {
        }

        public BPlusInternalNode(ObjectCache objectCache, int degree, string region = null)
            : base(objectCache, degree, region)
        {
        }

        public BPlusInternalNode(ObjectCache objectCache, string key, string region = null)
            : base(objectCache, key, region)
        {
        }

        public BPlusInternalNode(ObjectCache objectCache, string region = null)
            : base(objectCache, region)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();
            _childNodes = new List<string>();
        }

        public override BPlusNodeType NodeType
        {
            get { return BPlusNodeType.Internal; }
        }
    }
}
