using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.IO
{
    [Serializable]
    public class SegmentStreamHeader
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the size of the segment.
        /// </summary>
        /// <value>
        /// The size of the segment.
        /// </value>
        public int SegmentSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentStreamHeader"/> class.
        /// </summary>
        public SegmentStreamHeader(int segmentSize)
        {
            this.SegmentSize = segmentSize;
        }
    }
}
