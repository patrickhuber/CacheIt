using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using System.Collections.ObjectModel;

namespace CacheIt
{
    /// <summary>
    /// Defines an abstract class implementation of CacheEntryChangeMonitor
    /// </summary>
    public abstract class CacheEntryChangeMonitorBase : CacheEntryChangeMonitor
    {
        protected IList<string> cacheKeys;
        protected DateTimeOffset lastModified;
        protected string regionName;
        protected string uniqueId;

        public CacheEntryChangeMonitorBase(IEnumerable<string> keys, string regionName=null)
        {
            this.cacheKeys = new List<string>(keys);
            this.regionName = regionName;
        }

        public override ReadOnlyCollection<string> CacheKeys
        {
            get { return new ReadOnlyCollection<string>(cacheKeys); }
        }

        public override DateTimeOffset LastModified
        {
            get { return lastModified; }
        }

        public override string RegionName
        {
            get { return regionName; }
        }

        protected override void Dispose(bool disposing)
        {
        }

        public override string UniqueId
        {
            get { return uniqueId; }
        }
    }
}
