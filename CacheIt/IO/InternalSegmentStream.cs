using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.IO
{
    public class InternalSegmentStream : Stream
    {
        private ObjectCache _cache;
        private RegionKey _regionKey;

        private bool _canRead;
        private bool _canSeek;
        private bool _canWrite;
        private readonly int _segmentSize;
        private long _position;

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

        public override bool CanRead
        {
            get { return _canRead; }
        }

        public override bool CanSeek
        {
            get { return _canSeek; }
        }

        public override bool CanWrite
        {
            get { return _canWrite; }
        }

        public override void Flush()
        {            
        }

        public override long Length
        {
            get
            {
                SegmentStreamHeader header = Header;
                long fileSize = header.Length;                
                return fileSize;
            }
        }

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
