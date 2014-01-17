using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.Index
{
    public sealed class IndexTree<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        private RegionKey _regionKey;
        private ObjectCache _cache;

        public string CacheKey { get { return _regionKey.Key; } }
        public string CacheRegion { get { return _regionKey.Region; } }
        public int Order { get; private set; }

        private class IndexTreeHeader
        {
            public string RootAddress { get; set; }
        }

        public IndexTree(ObjectCache cache, int order, string cacheKey, string cacheRegion=null)
        {
            Order = order;
            _regionKey = new RegionKey { Key = cacheKey, Region = cacheRegion };
            _cache = cache;
            var header = Header;

            // first runthrough careate the header and root records
            if (header == null)
            {
                header = new IndexTreeHeader { RootAddress = Guid.NewGuid().ToString() };
                Header = header;
                var root = new IndexLeafNode<TKey, TPointer>(order, header.RootAddress, CacheRegion);
                _cache.Set(header.RootAddress, root, cacheRegion);
            }
        }
        
        public bool Insert(TKey key, TPointer value)
        {
            var header = Header;
            var root = Load(header.RootAddress, CacheRegion);
            var result = InsertRecursive(key, value, root, 0);

            if (root.IsFull())
            {
                var newRoot = new IndexInternalNode<TKey, TPointer>(Order, Guid.NewGuid().ToString(), CacheRegion);
                if (root.IsLeaf())
                {                    
                    var rootLeaf = root as IndexLeafNode<TKey, TPointer>;
                    var overflow = new IndexLeafNode<TKey, TPointer>(Order, Guid.NewGuid().ToString(), CacheRegion);

                    Split(newRoot, rootLeaf, overflow, true);
                    header.RootAddress = newRoot.Address;

                    // TODO: make this less volitile?
                    Save(overflow.Address, overflow);
                    Save(rootLeaf.Address, rootLeaf);
                    Save(newRoot.Address, newRoot);                    
                    Header = header;
                }
                else 
                {                
                    var rootInternal = root as IndexInternalNode<TKey, TPointer>;
                    var overflow = new IndexInternalNode<TKey, TPointer>(Order, Guid.NewGuid().ToString(), CacheRegion);

                    Split(newRoot, rootInternal, overflow, true);
                    header.RootAddress = newRoot.Address;
                    
                    //TODO: make this less volitile?
                    Save(overflow.Address, overflow);
                    Save(rootInternal.Address, overflow);
                    Save(newRoot.Address, newRoot);
                    Header = header;
                }
            }
            
            return result;
        }

        private void Split(IndexInternalNode<TKey, TPointer> parent, IndexInternalNode<TKey, TPointer> child, IndexInternalNode<TKey, TPointer> overflow, bool parentIsRoot)
        {
            child.Split(overflow);

            if (parentIsRoot)
            {
                var largest = child.GetLargestKey();
                child.Keys.Remove(largest);
                parent.Insert(largest, child.Address);
                parent.Insert(largest, overflow.Address);
            }
            else
            {
                var middleKey = overflow.Keys.Min();
                parent.Insert(middleKey, child.Address);
                parent.Insert(middleKey, overflow.Address);
            }
        }

        private void Split(IndexInternalNode<TKey, TPointer> parent, IndexLeafNode<TKey, TPointer> child, IndexLeafNode<TKey, TPointer> overflow, bool parentIsRoot)
        {        
            child.Split(overflow);
            var middleKey = overflow.Keys.Min();
            // if the parent is the root node, the child won't be included
            if(parentIsRoot)
                parent.Insert(middleKey, child.Address);
            parent.Insert(middleKey, overflow.Address);
        }

        private bool InsertRecursive(TKey key, TPointer pointer, IndexNode<TKey, TPointer> node, int level)
        {
            if (node.IsLeaf())
            {
                var leafNode = node as IndexLeafNode<TKey, TPointer>;
                return leafNode.Insert(key, pointer) > 0;
            }

            var index = node.FindIndex(key);            
            var internalNode = node as IndexInternalNode<TKey, TPointer>;
            string childAddress = internalNode.ChildAddressList[index];
            var childNode = Load(childAddress, CacheRegion);

            var result = InsertRecursive(key, pointer, childNode, level + 1);

            if (childNode.IsFull())
            {
                if (childNode.IsLeaf())
                {
                    var childLeafNode = childNode as IndexLeafNode<TKey, TPointer>;
                    var splitTo = new IndexLeafNode<TKey, TPointer>(Order, Guid.NewGuid().ToString(), CacheRegion);
                    result = childLeafNode.Split(splitTo);

                    // get the smallest key from the splitTo node and insert it in the current node
                    var splitToKey = splitTo.Keys.Min();
                    var splitToAddress = splitTo.Address;
                    internalNode.Insert(splitToKey, splitToAddress);
                    Save(splitTo.Address, splitTo);
                }
                else 
                {
                    var childInternalNode = childNode as IndexInternalNode<TKey, TPointer>;
                    var splitTo = new IndexInternalNode<TKey, TPointer>(Order, Guid.NewGuid().ToString(), CacheRegion);
                    result = childInternalNode.Split(splitTo);

                    var splitToKey = splitTo.Keys.Min();
                    var splitToAddress = splitTo.Address;
                    internalNode.Insert(splitToKey, splitToAddress);
                    Save(splitTo.Address, splitTo);
                }
            }

            return result;
        }

        private IndexTreeHeader Header
        {
            get { return _cache.Get(_regionKey.Key, _regionKey.Region) as IndexTreeHeader; }
            set { _cache.Set(CacheKey, value, CacheRegion); }
        }

        private IndexNode<TKey, TPointer> Load(string address, string region )
        {
            return _cache.Get(address, CacheRegion) as IndexNode<TKey, TPointer>;
        }

        private void Save(string address, IndexNode<TKey, TPointer> node)
        {
            _cache.Set(address, node, CacheRegion); 
        }
    }

    public abstract class IndexNode<TKey, TPointer>
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        public IList<TKey> Keys { get; protected set; }

        public int Order { get; protected set; }

        public int MinimumCapacity { get { return (Order - 1) / 2; } }

        public string Address { get { return regionKey.Key; } }
        
        public string Region { get { return regionKey.Region; } }

        protected RegionKey regionKey;

        protected IndexNode(int order, string address, string region = null)
        {
            Order = order;
            Keys = new List<TKey>();
            regionKey = new RegionKey { Key = address, Region = region };
        }        

        public bool IsFull()
        {
            return Keys.Count >= Order;
        }

        public int FindIndex(TKey key)
        {
            var index = Keys.Count;
            for (int i = 0; i < Keys.Count; i++)
            {
                if (key.CompareTo(Keys[i]) < 0)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public abstract bool IsLeaf();

        public TKey GetLargestKey()
        {
            return Keys.Max();
        }
    }

    public sealed class IndexInternalNode<TKey, TPointer> : IndexNode<TKey, TPointer>        
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        public IList<string> ChildAddressList { get; private set; }
        
        public IndexInternalNode(int order, string address, string region = null)
            : base(order, address, region)
        {
            ChildAddressList = new List<string>();
        }

        public int Insert(TKey key, string address)
        {
            var index = Keys.IndexOf(key);
            if (index < 0)
            {
                index = FindIndex(key);
                Keys.Insert(index, key);
            }

            if (key.CompareTo(Keys.Max()) == 0)
                ChildAddressList.Add(address);
            else
                ChildAddressList.Insert(index, address);

            return index;
        }

        public bool Split(IndexInternalNode<TKey, TPointer> to)
        {
            if (!IsFull())
                return false;

            int midpoint = (this.Keys.Count + 1) / 2;
            int newKeyCount = Keys.Count - midpoint;

            if (newKeyCount > Order || newKeyCount < MinimumCapacity)
                return false;

            while (midpoint < Keys.Count)
            {
                to.Keys.Add(Keys[midpoint]);
                Keys.RemoveAt(midpoint);
                to.ChildAddressList.Add(ChildAddressList[midpoint]);
                ChildAddressList.RemoveAt(midpoint);
            }
            if (to.Keys.Count + 1 < to.ChildAddressList.Count)
            {
                to.ChildAddressList.Add(ChildAddressList[ChildAddressList.Count - 1]);
                ChildAddressList.RemoveAt(ChildAddressList.Count - 1);
            }
            return true;
        }

        public bool Merge(IndexInternalNode<TKey, TPointer> from)
        {
            throw new NotImplementedException();
        }

        public override bool IsLeaf()
        {
            return false;
        }
    }

    public sealed class IndexLeafNode<TKey, TPointer> : IndexNode<TKey, TPointer>        
        where TKey : IComparable<TKey>
        where TPointer : IEquatable<TPointer>
    {
        public IList<IList<TPointer>> PointerIndex { get; private set; }
        public string NextAddress { get; private set; }

        public IndexLeafNode(int order, string address, string region = null)
            : base(order, address, region)
        {
            PointerIndex = new List<IList<TPointer>>();
        }

        public override bool IsLeaf()
        {
            return true;
        }

        public int Insert(TKey key, TPointer pointer)
        {
            // check for the key to see if we need to insert into the node
            var index = Keys.IndexOf(key);

            if (index < 0)
            {
                // insertion sort
                index = FindIndex(key);
                Keys.Insert(index, key);
                PointerIndex.Insert(index, new List<TPointer>());
            }

            // add pointer if not duplicate
            var pointerList = PointerIndex[index];
            if (!pointerList.Contains(pointer))
                pointerList.Add(pointer);

            // return the index we updated
            return index;
        }

        public bool Split(IndexLeafNode<TKey, TPointer> to)
        {
            if (!IsFull())
                return false;
            
            int midpoint = (this.Keys.Count) / 2;
            int newKeyCount = Keys.Count - midpoint;
            
            if (newKeyCount > Order || newKeyCount < MinimumCapacity)
                return false;
                        
            while (midpoint != Keys.Count)
            {
                to.Keys.Add(Keys[midpoint]);
                Keys.RemoveAt(midpoint);
                to.PointerIndex.Add(PointerIndex[midpoint]);
                PointerIndex.RemoveAt(midpoint);
            }

            to.NextAddress = this.NextAddress;
            this.NextAddress = to.Address;

            return true;
        }

        public bool Merge(IndexLeafNode<TKey, TPointer> from)
        {
            if (Keys.Count + from.Keys.Count > Order - 1)
                return false;

            while (from.Keys.Count > 0)
            {
                Keys.Add(from.Keys[0]);
                from.Keys.RemoveAt(0);
                PointerIndex.Add(from.PointerIndex[0]);
                from.PointerIndex.RemoveAt(0);
            }
            NextAddress = from.NextAddress;
            return true;
        }
    }
}
