using CacheIt.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// A special flavor of the SegmentStream class that disables the disposing checks because lucene does a lot of invalid stream handling.
    /// </summary>
    public class LuceneSegmentStream : SegmentStream
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LuceneSegmentStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public LuceneSegmentStream(ObjectCache objectCache, string key, string region = null) 
            : base(objectCache, key, region) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LuceneSegmentStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="segmentSize">Size of the buffer.</param>
        /// <param name="region">The region.</param>
        public LuceneSegmentStream(ObjectCache objectCache, string key, int segmentSize, string region = null) 
            : base(objectCache, key, segmentSize, region) { }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead { get { return true; } }
        
        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite { get { return true; } }
        
        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek { get { return true; } }
    }
}
