using System;
namespace CacheIt.IO
{
    /// <summary>
    /// 
    /// </summary>
    public interface IChunkStreamHeader
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        long Length { get; set; }

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// </summary>
        /// <value>
        /// The size of the buffer.
        /// </value>
        int BufferSize { get; set; }
    }
}
