using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Text;

namespace CacheIt.Index
{
    [Serializable]
    public abstract class BPlusNode<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        public const int MinimumDegree = 2;
        public const int DefaultDegree = 100;

        protected ObjectCache cache;
        protected RegionKey regionKey;
        protected IList<TKey> keys;
        protected readonly int degree;

        public BPlusNode(ObjectCache objectCache, int degree, string region = null)
            : this(objectCache, degree, Guid.NewGuid().ToString(), region)
        { 
        }

        public BPlusNode(ObjectCache objectCache, int degree, string key, string region = null)
        {
            if (degree < MinimumDegree)
                throw new ArgumentException("Argument provided is less than the minimum allowed degree.", "degree");
            this.degree = degree;
            cache = objectCache;
            regionKey = new RegionKey { Key = key, Region = region };
            Initialize();
        }

        public BPlusNode(ObjectCache objectCache, string region = null)
            : this(objectCache, DefaultDegree, region)
        { }

        public BPlusNode(ObjectCache objectCache, string key, string region = null)
            : this(objectCache, DefaultDegree, key, region)
        { }

        protected virtual void Initialize()
        {
            keys = new List<TKey>();
        }

        public abstract BPlusNodeType NodeType { get; }
        
        public BPlusNode<TKey, TPointer> Search(TKey key)
        {
            if (NodeType == BPlusNodeType.Leaf)
                return this;

        }
    }
}
