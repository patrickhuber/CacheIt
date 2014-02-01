using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CacheIt.Web
{
    public class WebCacheChangeMonitor : CacheEntryChangeMonitorBase
    {
        private WebCache webCache;

        public WebCacheChangeMonitor(ReadOnlyCollection<string> keys, string regionName, WebCache cache)
            : base(keys, regionName)
        {
            webCache = cache;
            webCache.OnCacheItemRemoved += cache_OnCacheItemRemoved;
        }

        void cache_OnCacheItemRemoved(string key, object value, System.Web.Caching.CacheItemRemovedReason reason)
        {
            if (cacheKeys.Contains(key))
                this.OnChanged(reason);
        }
    }
}