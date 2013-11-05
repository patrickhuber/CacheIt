using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt
{
    /// <summary>
    /// Defines a region and Key
    /// </summary>
    public class RegionKey
    {
        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get; set; }
    }
}
