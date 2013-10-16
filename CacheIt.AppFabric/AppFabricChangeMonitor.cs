using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.AppFabric
{
    /// <summary>
    /// 
    /// </summary>
    public class AppFabricChangeMonitor : CacheEntryChangeMonitorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppFabricChangeMonitor"/> class.
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <param name="regionName">Name of the region.</param>
        public AppFabricChangeMonitor(IEnumerable<string> keys, string regionName = null)
            : base(keys, regionName) 
        { }
    }
}
