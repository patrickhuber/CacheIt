using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CacheIt.IO
{
    /// <summary>
    /// Defines a header for the chunk stream. The is the minimal information necessary to read the stream.
    /// </summary>
    [Serializable]
    public class ChunkStreamHeader : IChunkStreamHeader
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public long Length { get; set; }

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// </summary>
        /// <value>
        /// The size of the buffer.
        /// </value>
        public int BufferSize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkStreamHeader"/> class.
        /// </summary>
        /// <param name="bufferSize">Size of the buffer.</param>
        public ChunkStreamHeader(int bufferSize)
        {
            BufferSize = bufferSize;
            Length = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkStreamHeader"/> class.
        /// </summary>
        /// <param name="header">The header.</param>
        public ChunkStreamHeader(IChunkStreamHeader header)
        {
            BufferSize = header.BufferSize;
            Length = header.Length;
        }
    }
}
