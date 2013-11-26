using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.Collections
{
    public class CacheLinkedList<T> : IList<T>
    {
        private RegionKey _regionKey;
        private ObjectCache _cache;

        private class DistributedLinkedListHeader
        {
            public int Count { get; set; }
            public string HeadPointer { get; set; }
        }

        public CacheLinkedList(ObjectCache cache, string key, string region = null)
        {
            _cache = cache;
            _regionKey = new RegionKey { Key = key, Region = region };
        }

        #region IList<T>
        public int IndexOf(T item)
        {
            int i = 0;
            foreach (var cacheNode in Enumerate())
            {
                if (cacheNode.Data.Equals(item))
                    return i;
                i++;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public T this[int index]
        {
            get
            {
                var value = GetNode(index);
                if (value == null)
                    return default(T);
                if (value.Data == null)
                    return default(T);
                return value.Data;
            }
            set
            {                
                int i = 0;
                foreach (var item in Enumerate())
                {
                    i++;
                }
            }
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (var item in Enumerate())
            {
                array[index] = item.Data;
                index++;
            }
        }

        public int Count
        {
            get 
            {
                return Header.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            var node = FindNode(item);
            if (node == null)
                return false;
            
            var header = Header;
            if (header.Count == 0)
                return false;

            CacheLinkedListNode<T> next = null;
            CacheLinkedListNode<T> previous = null;
            if (node.HasNext())
            {
                next = _cache.Get(node.Next, _regionKey.Region) as CacheLinkedListNode<T>; 
            }
            if (node.HasPrevious())
            {
                previous = _cache.Get(node.Previous, _regionKey.Region) as CacheLinkedListNode<T>;                
            }

            // if true, this is the head node
            if (previous == null && next != null)
            {                
                _cache.Remove(header.HeadPointer);
                header.HeadPointer = node.Next;
                Header = header;                
            }
            // if true, this is the tail node
            else if (previous != null && next == null)
            {
                _cache.Remove(previous.Next);
                previous.Next = null;
                _cache.Set(node.Previous, previous, _regionKey.Region);
            }
            // if true, this is a middle node
            else if (previous != null && next != null)
            {
                _cache.Remove(previous.Next, _regionKey.Region);
                previous.Next = node.Next;
                next.Previous = node.Previous;
                _cache.Set(node.Previous, previous, _regionKey.Region);
                _cache.Set(node.Next, next, _regionKey.Region);
            }

            header.Count -= 1;
            _cache.Set(header.HeadPointer, next, _regionKey.Region);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var cacheNode in Enumerate())
                yield return cacheNode.Data;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IList<T>

        private CacheLinkedListNode<T> GetNode(int ordinal)
        {
            int index = 0;
            foreach (var node in Enumerate())
            {
                if (index == ordinal)
                    return node;
                index++;
            }
            return null;
        }

        private CacheLinkedListNode<T> FindNode(T item)
        {
            foreach (var node in Enumerate())
                if (node.Data.Equals(item))
                    return node;
            return null;
        }

        private IEnumerable<CacheLinkedListNode<T>> Enumerate()
        {            
            string key = _regionKey.Key;
            do
            {
                var current = _cache.Get(_regionKey.Key, _regionKey.Region) as CacheLinkedListNode<T>;
                if (current == null)                
                    yield break;                    
                
                yield return current;
                key = current.Next;
            }
            while (!string.IsNullOrWhiteSpace(key));
            yield break;
        }

        private DistributedLinkedListHeader Header
        {
            get
            {
                return _cache.Get(_regionKey.Key, _regionKey.Region) as DistributedLinkedListHeader;
            }
            set
            {
                _cache.Set(_regionKey.Key, value, _regionKey.Key);
            }
        }
    }
}
