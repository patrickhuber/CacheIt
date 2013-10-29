using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Represents a cache file object used to store metadata about a file
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
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the size information bytes.
        /// </summary>
        /// <value>
        /// The size information bytes.
        /// </value>
        public long SizeInBytes { get; set; }
    }
}
