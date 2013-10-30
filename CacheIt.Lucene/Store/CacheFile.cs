using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Represents a cache file object used to store metadata about a file. Similar to INodes in the unix file system.
    /// </summary>
    public class CacheFile
    {
        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>
        /// The last modified.
        /// </value>
        public long LastModified { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Identifier { get; set; }
    }
}
