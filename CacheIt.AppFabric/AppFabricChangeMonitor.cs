using Microsoft.ApplicationServer.Caching;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    /// <summary>
    /// Provides an AppFabricChangeMonitor
    /// <see cref="http://msdn.microsoft.com/en-us/library/hh334296(v=azure.10).aspx"/>
    /// </summary>
    public sealed class AppFabricChangeMonitor : CacheEntryChangeMonitorBase, IDisposable
    {
        private DataCache dataCache;
        private IList<DataCacheNotificationDescriptor> notificationDescriptorList;

        public AppFabricChangeMonitor(ReadOnlyCollection<string> keys, string regionName, DataCache dataCache)
            : base(keys, regionName) 
        {
            this.dataCache = dataCache;            
            this.notificationDescriptorList = new List<DataCacheNotificationDescriptor>();

            foreach (var key in keys)
            {
                var notificationDescriptor = 
                    this.dataCache.AddItemLevelCallback(
                        key, 
                        DataCacheOperations.RemoveItem 
                            | DataCacheOperations.ReplaceItem 
                            | DataCacheOperations.AddItem, 
                        ItemChangedCallbackDelegate);
                notificationDescriptorList.Add(notificationDescriptor);
            }
        }

        private void ItemChangedCallbackDelegate(
            string cacheName, 
            string region, 
            string key, 
            DataCacheItemVersion version, 
            DataCacheOperations operation, 
            DataCacheNotificationDescriptor descriptior)
        {
            if(cacheKeys.Contains(key) && region == regionName)
                this.OnChanged(null);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing || notificationDescriptorList.Count == 0)
                return;
            foreach (var notificationDescriptor in notificationDescriptorList)
                dataCache.RemoveCallback(notificationDescriptor);
        }
    }
}
