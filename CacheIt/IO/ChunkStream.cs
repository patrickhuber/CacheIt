using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Caching;

namespace CacheIt.IO
{
    /// <summary>
    /// Defines a cache stream that writes and reads data in chunks.
    /// </summary>
    public class ChunkStream : Stream
    {
        private bool canRead;
        private bool canSeek;
        private bool canWrite;
        private ChunkStreamHeader header;
        private byte[] localBuffer;
        private ObjectCache cache;
        private string key;
        private string region;
        private long position;

        /// <summary>
        /// The default buffer size
        /// </summary>
        public static readonly int DefaultBufferSize = 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="header">The header.</param>
        public ChunkStream(ObjectCache objectCache, string key, string region, int bufferSize)
        {
            cache = objectCache;
            this.key = key;
            this.region = region;
            localBuffer = new byte[bufferSize];

            // create the header if it doesn't exist
            header = objectCache.Get(key, region, ()=>new ChunkStreamHeader(bufferSize));

            canRead = true;
            canSeek = true;
            canWrite = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        public ChunkStream(ObjectCache objectCache, string key, string region)
            : this(objectCache, key, region, DefaultBufferSize)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        public ChunkStream(ObjectCache objectCache, string key)
            : this(objectCache, key, (string)null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        public ChunkStream(ObjectCache objectCache, string key, int bufferSize)
            : this(objectCache, key, (string)null, bufferSize)
        { }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>
        /// The header.
        /// </value>
        public IChunkStreamHeader Header { get { return this.header; } }

        #region Stream
        
        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead { get { return canRead; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek { get { return canSeek; } }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite { get { return canWrite; } }
        
        /// <summary>
        /// Gets the length in bytes of the stream.
        /// </summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long Length { get { return this.header.Length; } }

        /// <summary>
        /// Gets or sets the position within the current stream.
        /// </summary>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        public override long Position
        {
            get { return position; }
            set
            {
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Flush()
        {   
            // generate the buffer key and save the current buffer
            var key = GenerateBufferKey(position);
            cache.Set(key, localBuffer, region);

            // save the file header as well
            SaveHeader();
        }

        /// <summary>
        /// Sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void SetLength(long value)
        {
            // are we truncating the stream?
            if (this.Header.Length > value)
            {
                int valueIndex = GetBufferIndex(value);
                int tailBufferIndex = GetBufferIndex(this.Header.Length);

                // compute all of the buffer indicies between the value and the header                
                for (int bufferIndex = valueIndex; bufferIndex <= tailBufferIndex; bufferIndex++)
                {
                    // remove the buffers from the cache
                    cache.Remove(GenerateBufferKey(bufferIndex));
                }
            }

            if (position > value)
                position = value;
                        
            this.SaveHeader(value);
        }

        /// <summary>
        /// Sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new InvalidOperationException(strings.CacheIt_IO_SeekDisabledDisposing);
            
            // compute the absolute offset
            long absoluteOffset = 0;
            var lessThanZeroOffsetException = new ArgumentOutOfRangeException("Seeking to an aboslute position less than 0 is not supported.");

            // from the begining (0) add the offset
            if (origin == SeekOrigin.Begin)
            {
                if (offset < 0)
                    throw lessThanZeroOffsetException;
                absoluteOffset = offset;
            }
            // from the position (position) add the offset
            else if (origin == SeekOrigin.Current)
            {
                if ((position + offset) < 0)
                    throw lessThanZeroOffsetException;
                absoluteOffset = position + offset;
            }
            // from the end (Header.Length) add the offset
            else if (origin == SeekOrigin.End)
            {
                if ((this.Header.Length + offset) < 0)
                    throw lessThanZeroOffsetException;
                absoluteOffset = this.Header.Length + offset;
            }

            // if we are growing the size of the stream
            if (absoluteOffset > this.Header.Length)
            {
                this.SaveHeader(absoluteOffset);
            }

            // determine the current buffer index and the new buffer index
            string currentBufferKey = this.GenerateBufferKey(position);
            string newBufferKey = this.GenerateBufferKey(absoluteOffset);

            // if the indicies are different, we need to load the buffer at the index.
            if (currentBufferKey != newBufferKey)
            {                
                // save the current buffer
                cache.Set(currentBufferKey, localBuffer, region);

                // get the new buffer
                localBuffer = cache.Get(currentBufferKey, region, () => new byte[this.Header.BufferSize]);
            }

            position = absoluteOffset;

            return absoluteOffset;
        }

        /// <summary>
        /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new InvalidOperationException(strings.CacheIt_IO_ReadDisabledDisposing);
            if (buffer == null)
                throw new ArgumentNullException("buffer is null");
            if (buffer.Length < (offset + count))
                throw new ArgumentException("buffer size is smaller than the offset + length");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset is negative");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count is negative");

            // quick check to see if we are at the end of the stream
            if (position > this.header.Length)
                return 0;
            
            // set the actual count
            int actualCount = count;
            if (count > this.header.Length)
                actualCount = (int)this.header.Length;

            // setup some indicies, we are going to iterate through the stream using buffer chunks instead of individual
            // byte addresses. 
            int endBufferIndex = GetBufferIndex(position + actualCount);
            int startBufferIndex = GetBufferIndex(position);
            int bytesRead = 0;

            // for durability we will copy the position into a currentPosition variable
            // the Stream class requires that the position not change if there is an error
            long currentPosition = position;
                        
            // loops through the buffers between the current position and the target position
            for (int bufferIndex = startBufferIndex; bufferIndex <= endBufferIndex; bufferIndex++)
            {
                // Do a clean read of the current buffer.
                // Make sure we aren't at the end of the file so we don't allocate useless buffers.                
                if (currentPosition < this.Header.Length)
                {
                    string currentBufferKey = this.GenerateBufferKey(currentPosition);
                    this.localBuffer = this.localBuffer = cache.Get(currentBufferKey, region, () => new byte[this.Header.BufferSize]);
                }
                else { break; }

                // the relative position is where to start reading in the current buffer
                int relativePosition = GetRelativeOffset(currentPosition, bufferIndex);

                // if the current buffer index != the end buffer index
                // set the relative count to buffersize - relative count
                // otherwise set the relative count to the count - bytes read
                int relativeCount = bufferIndex < endBufferIndex
                    ? this.Header.BufferSize - relativePosition
                    : actualCount - bytesRead;

                // copy the bytes to the input buffer
                Array.Copy(this.localBuffer, relativePosition, buffer, bytesRead + offset, relativeCount);

                // increment both the bytes read and the current position
                bytesRead += relativeCount;
                currentPosition += relativeCount;
            }

            // we can only increment the position when the operation completes successfully
            position = currentPosition;

            // return the total number of bytes read
            return bytesRead;
        }

        /// <summary>
        /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="buffer">An array of bytes. This method copies <paramref name="count" /> bytes from <paramref name="buffer" /> to the current stream.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new InvalidOperationException(strings.CacheIt_IO_WriteDisabledDisposing);
            int endBufferIndex = GetBufferIndex(position + count);
            int startBufferIndex = GetBufferIndex(position);
            int bytesWritten = 0;

            // for durability the position must not advance
            long currentPosition = position;

            // loops through the buffers between the current position and the target position
            for (int bufferIndex = startBufferIndex; bufferIndex <= endBufferIndex; bufferIndex++)
            {
                // if we are not in the first iteration
                if (bufferIndex != startBufferIndex)
                {
                    // save the previous buffer
                    string currentBufferKey = GenerateBufferKey(bufferIndex - 1);
                    cache.Set(currentBufferKey, this.localBuffer, region);

                    // load the next buffer, creating it if necessary
                    currentBufferKey = GenerateBufferKey(bufferIndex);
                    this.localBuffer = cache.Get(currentBufferKey, region, () => new byte[this.Header.BufferSize]);
                }

                // the relative position is where to start writing in the current buffer
                // only non-zero when doing the first iteration
                int relativeOffset = GetRelativeOffset(currentPosition, bufferIndex);

                // if the current buffer index != the end buffer index
                // set the relative count to buffersize - relative count
                // otherwise set the relative count to the count - bytes written
                int relativeCount = bufferIndex < endBufferIndex
                    ? this.Header.BufferSize - relativeOffset
                    : count - bytesWritten;

                // copies bytes from the write buffer to the current buffer
                Array.Copy(buffer, offset + bytesWritten, this.localBuffer, relativeOffset, relativeCount);

                // increment the counters
                bytesWritten += relativeCount;
                currentPosition += relativeCount;
            }

            // set the position
            position = currentPosition;

            // set the header length if the position is greater than the previous size
            if (position > this.header.Length)
            {                
                this.SaveHeader(position);
            }
        }

        #endregion Stream

        #region IDisposable

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.Stream" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.canRead = false;
            this.canWrite = false;
            this.canSeek = false;
            Flush();
            base.Dispose(disposing);
        }

        #endregion IDisposable

        /// <summary>
        /// Gets the relative offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        protected int GetRelativeOffset(long offset)
        {
            var bufferIndex = GetBufferIndex(offset);
            return GetRelativeOffset(offset, bufferIndex);
        }

        /// <summary>
        /// Gets the relative offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="bufferIndex">Index of the buffer.</param>
        /// <returns></returns>
        protected int GetRelativeOffset(long offset, int bufferIndex)
        {
            // compute the buffer relative offset, this is the index of the stream offset within the buffer
            // Examples:
            // ---------
            // offset =    0,  bufferSize = 1024    {    0 - (0 * 1024) =   0 }
            // offset = 2000,  bufferSize = 1024    { 2000 - (1 * 1024) = 976 }
            // offset = 2049,  bufferSize = 1024    { 2049 - (2 * 1024) =   1 }
            // offset = 3000,  bufferSize = 2048    { 3000 - (1 * 2048) = 952 }
            return (int)(offset - ((long)bufferIndex * (long)Header.BufferSize));
        }

        /// <summary>
        /// Gets the index of the buffer.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected int GetBufferIndex(long offset)
        {
            // get the buffer index that contains the offset
            // Examples:
            // ---------
            // offset =    0, bufferSize = 1024     { 0 -1      / 1024 = 0       }
            // offset = 1024, bufferSize = 1024     { 1024 - 1  / 1024 = -1 -> 0 }
            // offset = 2000, bufferSize = 1024     { 2000 - 1  / 1024 = 1       }
            // offset = 2049, bufferSize = 1024     { 2049 - 1  / 1024 = 2       }
            // offset - 3000, bufferSize = 2048     { 3000 - 1  / 2048 = 1       }
            var bufferIndex = (int)((offset - 1L) / (long)header.BufferSize);
            return bufferIndex < 0 ? 0 : bufferIndex;
        }

        /// <summary>
        /// Generates the buffer key.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        protected string GenerateBufferKey(long offset)
        {
            var bufferIndex = GetBufferIndex(offset);

            return GenerateBufferKey(bufferIndex);
        }

        /// <summary>
        /// Generates the buffer key.
        /// </summary>
        /// <param name="bufferIndex">Index of the buffer.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual string GenerateBufferKey(int bufferIndex)
        {
            return string.Format("{0}_{1}", key, bufferIndex);
        }

        /// <summary>
        /// Saves the header.
        /// </summary>
        protected virtual void SaveHeader()
        { 
            cache.Set(key, this.header, region);            
        }

        /// <summary>
        /// Saves the header.
        /// </summary>
        /// <param name="length">The length.</param>
        protected virtual void SaveHeader(long length)
        {
            this.header.Length = length;
            cache.Set(key, this.header, region);
        }
    }
}
