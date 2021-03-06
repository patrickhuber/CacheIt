﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.IO
{
    public class SegmentUtility 
    {
        /// <summary>
        /// Gets the index of the segment.
        /// </summary>
        /// <param name="absolutePosition">The position.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        public static int GetSegmentIndex(long absolutePosition, int bufferSize)
        {
            return Convert.ToInt32(absolutePosition / (long)bufferSize);
        }

        /// <summary>
        /// Gets the position in the segment.
        /// </summary>
        /// <param name="absolutePosition">The position.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        public static int GetPositionInSegment(long absolutePosition, int bufferSize)
        {
            var segmentIndex = GetSegmentIndex(absolutePosition, bufferSize);
            return (int)(absolutePosition - ((long)segmentIndex * (long)bufferSize));
        }

        /// <summary>
        /// Generates the segment key.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GenerateSegmentKey(int segmentIndex, string key)
        {
            return string.Format("{0}_{1}", key, segmentIndex);
        }
    }
}
