using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.IO
{
    /// <summary>
    /// An internal implementation used to send and read data from the cache as segments. This stream is unbuffered so users should not use it directly.
    /// </summary>
    internal class InternalSegmentStream : Stream
    {
        private ObjectCache _cache;
        private RegionKey _regionKey;

        private bool _canRead;
        private bool _canSeek;
        private bool _canWrite;
        private readonly int _segmentSize;
        private long _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSegmentStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="segmentSize">Size of the segment.</param>
        /// <param name="region">The region.</param>
        public InternalSegmentStream(ObjectCache objectCache, string key, int segmentSize, string region)
        {
            _segmentSize = segmentSize;
            _cache = objectCache;
            _regionKey = new RegionKey { Key = key, Region = region };
            _canRead = true;
            _canWrite = true;
            _canSeek = true;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key { get { return String.Copy(_regionKey.Key); } }

        /// <summary>
        /// Gets the region.
        /// </summary>
        /// <value>
        /// The region.
        /// </value>
        public string Region { get { return _regionKey.Region != null ? String.Copy(_regionKey.Region) : null; } }
        
        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <returns></returns>
        private SegmentStreamHeader Header
        {
            get
            {
                return _cache.Get<SegmentStreamHeader>(Key, () => new SegmentStreamHeader(_segmentSize), _regionKey.Region);
            }
            set
            {
                _cache.Set(Key, value, Region);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports reading.
        /// </summary>
        /// <returns>true if the stream supports reading; otherwise, false.</returns>
        public override bool CanRead
        {
            get { return _canRead; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        /// <returns>true if the stream supports seeking; otherwise, false.</returns>
        public override bool CanSeek
        {
            get { return _canSeek; }
        }

        /// <summary>
        /// Gets a value indicating whether the current stream supports writing.
        /// </summary>
        /// <returns>true if the stream supports writing; otherwise, false.</returns>
        public override bool CanWrite
        {
            get { return _canWrite; }
        }

        /// <summary>
        /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {            
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        public override long Length
        {
            get
            {
                SegmentStreamHeader header = Header;
                long fileSize = header.Length;                
                return fileSize;
            }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <returns>The current position within the stream.</returns>
        /// <exception cref="System.InvalidOperationException">Seek is disabled while disposing.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Position set value must be non negative</exception>
        public override long Position
        {
            get
            {
                if (!CanSeek)
                    throw new InvalidOperationException("Seek is disabled while disposing.");
                return _position;
            }
            set
            {
                if (value < 0L)
                    throw new ArgumentOutOfRangeException("value", "Position set value must be non negative");               
                Seek(value, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Reads the specified array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        public override int Read(byte[] array, int offset, int count)
        {
            // calculate the actual count based on the current length
            var streamLength = Header.Length;
            var endPosition = _position + count;
            if (endPosition > streamLength)
            {
                endPosition = streamLength;
                count = (int)((long)streamLength - _position);
            }

            int startSegmentIndex = SegmentUtility.GetSegmentIndex(_position, _segmentSize);
            int endSegmentIndex = SegmentUtility.GetSegmentIndex(endPosition, _segmentSize);

            int bytesRead = 0;

            // loop through segments
            for (int segmentIndex = startSegmentIndex; segmentIndex <= endSegmentIndex; segmentIndex++)
            {
                string segmentKey = SegmentUtility.GenerateSegmentKey(segmentIndex, _regionKey.Key);
                var segmentPosition = SegmentUtility.GetPositionInSegment(bytesRead + _position, _segmentSize);

                // calculate the byteCount
                var byteCount = _segmentSize - segmentPosition;
                if (count - bytesRead < byteCount)
                    byteCount = (int)(count - (long)bytesRead);

                // are there bytes to read?
                if (byteCount > 0)
                {
                    // fetch the cache segment from cache
                    var segment = _cache.Get(segmentKey, _regionKey.Region) as byte[];
                    if (segment == null)
                        throw new InvalidOperationException(string.Format("Unable to read segment {0} from the Cache.", segmentKey));

                    // copy the segment from cache onto the array
                    System.Array.Copy(segment, segmentPosition, array, offset + bytesRead, byteCount);
                    bytesRead += byteCount;
                }
            }
            _position += (long)bytesRead;
            return bytesRead;
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            var length = Header.Length;
            var position = offset;
            if (origin == SeekOrigin.End)
                position = length - offset;
            if (origin == SeekOrigin.Current)
                position = _position + offset;
            _position = position;
            var header = this.Header;

            // adjust the header length
            if (header.Length < position)
            {
                header.Length = position;
                this.Header = header;
            }
            return position;
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            long offset = _position;
            long length = Header.Length;

            // Set the physicial size for the specified segmenet collection to the current position
            // Can be used to truncate or extend the physicial size
            if (value > length)
                Extend(value, length);
            if (value < length)
                Truncate(value, length);

            if (offset == value)
                return;
            if (offset < value)
                Seek(offset, SeekOrigin.Begin);
            else
                Seek(0L, SeekOrigin.End);     
        }

        /// <summary>
        /// Extends the stream to the specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        protected virtual void Extend(long newLength, long currentLength)
        {
            if (newLength <= currentLength)
                return;
            var currentLengthSegmentIndex = SegmentUtility.GetSegmentIndex(currentLength - 1, _segmentSize);
            var newLengthSegmentIndex = SegmentUtility.GetSegmentIndex(newLength - 1, _segmentSize);

            // allocate segments between the current length and the target length
            for (int i = currentLengthSegmentIndex; i <= newLengthSegmentIndex; i++)
            {
                string key = SegmentUtility.GenerateSegmentKey(i, _regionKey.Key);
                _cache.Get(key, () => new byte[_segmentSize], Region);
            }
        }

        /// <summary>
        /// Truncates the stream to the specified length.
        /// </summary>
        /// <param name="length">The length.</param>
        protected virtual void Truncate(long newLength, long currentLength)
        {
            if (newLength >= currentLength)
                return;
            var currentLengthSegmentIndex = SegmentUtility.GetSegmentIndex(currentLength - 1, _segmentSize);
            var newLengthSegmentIndex = SegmentUtility.GetSegmentIndex(newLength - 1, _segmentSize);
            for (int i = currentLengthSegmentIndex; i > newLengthSegmentIndex; i--)
            {
                string key = SegmentUtility.GenerateSegmentKey(i, Key);
                _cache.Remove(key, Region);
            }
            if (newLength == 0)
            {
                string key = SegmentUtility.GenerateSegmentKey(0, Key);
                _cache.Remove(key, Region);
            }
        }

        /// <summary>
        /// Writes the specified array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public override void Write(byte[] array, int offset, int count)
        {
            int startSegmentIndex = SegmentUtility.GetSegmentIndex(_position, _segmentSize);
            int endSegmentIndex = SegmentUtility.GetSegmentIndex(_position + count, _segmentSize);

            int bytesWritten = 0;

            // loop through segments
            for (int segmentIndex = startSegmentIndex; segmentIndex <= endSegmentIndex; segmentIndex++)
            {
                // create the segment key
                string segmentKey = SegmentUtility.GenerateSegmentKey(segmentIndex, _regionKey.Key);

                // with the segment position we know where to start writing in the current segment
                var segmentPosition = SegmentUtility.GetPositionInSegment(bytesWritten + _position, _segmentSize);

                // calculate the byte count 
                var byteCount = _segmentSize - segmentPosition;
                if (count - bytesWritten < byteCount)
                    byteCount = count - bytesWritten;

                // if we have bytes to write, write the bytes
                if (byteCount > 0)
                {
                    // fetch or create the segment in the cache
                    var segment = _cache.Get<byte[]>(segmentKey, () => new byte[_segmentSize], _regionKey.Region);

                    System.Array.Copy(array, offset + bytesWritten, segment, segmentPosition, byteCount);
                    bytesWritten += byteCount;
                    _cache.Set(segmentKey, segment, _regionKey.Region);
                }
            }

            _position += count;
            var header = this.Header;
            if (_position > header.Length)
            {
                header.Length = _position;
                this.Header = header;
            }
        }
    }
}
