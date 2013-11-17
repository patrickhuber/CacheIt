using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace CacheIt.Collections
{
    /// <summary>
    /// Wraps methods that mutate state to provide persistence
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PersistentSet<T> : ISet<T>
    {
        private ObjectCache objectCache;
        private string key;
        private string region;
        private ISet<T> innerSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentSet{T}" /> class.
        /// </summary>
        /// <param name="innerSet">The inner set.</param>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public PersistentSet(ISet<T> innerSet, ObjectCache objectCache, string key, string region = null)
        {
            this.objectCache = objectCache;
            this.innerSet = innerSet;
            this.key = key;
            this.region = region;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistentSet{T}"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public PersistentSet(ObjectCache objectCache, string key, string region = null)
            : this(new HashSet<T>(), objectCache, key, region)
        { }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        protected virtual void Refresh()
        {
            this.innerSet = this.objectCache.Get(key, region) as ISet<T>;
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        protected virtual void Save()
        {
            this.objectCache.Set(key, innerSet, region);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Add(T item)
        {
            Refresh();
            bool result = this.innerSet.Add(item);
            Save();
            return result;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            Refresh();
            innerSet.ExceptWith(other);
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            Refresh();
            innerSet.IntersectWith(other);
        }

        public bool IsProperSubsetOf(IEnumerable<T> other)
        {
            Refresh();
            return innerSet.IsProperSubsetOf(other);
        }

        public bool IsProperSupersetOf(IEnumerable<T> other)
        {
            Refresh();
            return innerSet.IsProperSupersetOf(other);
        }

        public bool IsSubsetOf(IEnumerable<T> other)
        {
            Refresh();
            return innerSet.IsSubsetOf(other);
        }

        public bool IsSupersetOf(IEnumerable<T> other)
        {
            Refresh();
            return innerSet.IsSupersetOf(other);
        }

        public bool Overlaps(IEnumerable<T> other)
        {
            Refresh();
            return innerSet.Overlaps(other);
        }

        public bool SetEquals(IEnumerable<T> other)
        {
            Refresh();
            return innerSet.SetEquals(other);
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            Refresh();
            innerSet.SymmetricExceptWith(other);
        }

        public void UnionWith(IEnumerable<T> other)
        {
            Refresh();
            innerSet.UnionWith(other);
        }

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            this.innerSet.Clear();
            Save();
        }

        public bool Contains(T item)
        {
            Refresh();
            return innerSet.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Refresh();
            innerSet.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get 
            {
                Refresh();
                return innerSet.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                Refresh();
                return innerSet.IsReadOnly;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public bool Remove(T item)
        {
            Refresh();
            bool result = innerSet.Remove(item);
            Save();
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            Refresh();
            return innerSet.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            Refresh();
            return innerSet.GetEnumerator();
        }
    }
}
