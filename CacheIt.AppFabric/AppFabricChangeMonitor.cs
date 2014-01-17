using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AppFabricChangeMonitor : CacheEntryChangeMonitorBase
    {
        public AppFabricChangeMonitor(ReadOnlyCollection<string> keys, string regionName = null)
            : base(keys, regionName) 
        { 
            
        }     
    }
}
