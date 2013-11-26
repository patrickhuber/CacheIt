using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;

namespace CacheIt.Index
{
    public class BPlusLeafNode<TKey, TPointer> : BPlusNode<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        // this structure will handle duplicate keys through a sub BPlusTree structure
        private IList<IList<TPointer>> _pointers;
        private string _siblingNodeAddress;

        public BPlusLeafNode(ObjectCache objectCache, int degree, string key, string region = null)
            : base(objectCache, degree, key, region)
        {
        }

        public BPlusLeafNode(ObjectCache objectCache, int degree, string region = null)
            : base(objectCache, degree, region)
        {
        }

        public BPlusLeafNode(ObjectCache objectCache, string key, string region = null)
            : base(objectCache, key, region)
        {
        }

        public BPlusLeafNode(ObjectCache objectCache, string region = null)
            : base(objectCache, region)
        {
        }

        protected override void Initialize()
        {
 	         base.Initialize();
             _pointers = new List<IList<TPointer>>();
             for (int i = 0; i < degree; i++)
             {
                 _pointers.Add(new List<TPointer>());
             }
             _siblingNodeAddress = Guid.NewGuid().ToString();
        }

        public override BPlusNodeType NodeType
        {
            get { return BPlusNodeType.Leaf; }
        }

        public void Insert(TKey key, TPointer pointer)
        {
            int i = 0;
            for (; i < keys.Count; i++)
            {
                if (key.CompareTo(keys[i]) <= 0)
                {
                    break;
                }
            }
            
            var _pointerList = _pointers[i];
            
        }
    }
}
