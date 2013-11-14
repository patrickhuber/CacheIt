using CacheIt.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.IO
{
    /// <summary>
    /// A buffered segment stream that is based off of the .NET 4.0 FileStream buffered read/write approach.
    /// </summary>
    public class SegmentStream : Stream
    {
        private ObjectCache _cache;
        private RegionKey _regionKey;

        private bool _canRead;
        private bool _canSeek;
        private bool _canWrite;
        private readonly int _segmentSize;
        private byte[] _segment;
        private int _readPosition;
        private int _readLength;
        // the position within the current buffer of a write
        private int _writePosition;
        private long _position;

        public const int DefaultSegmentSize = 1024;
        public const string DefaultRegion = null;

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
        /// Initializes a new instance of the <see cref="SegmentStream"/> class.
        /// </summary>
        /// <param name="objectCache">The object cache.</param>
        /// <param name="key">The key.</param>
        /// <param name="region">The region.</param>
        /// <param name="segmentSize">Size of the buffer.</param>
        public SegmentStream(ObjectCache objectCache, string key, int segmentSize = DefaultSegmentSize, string region = DefaultRegion)
        {
            _segment = new byte[segmentSize];
            _segmentSize = segmentSize;
            _cache = objectCache;
            _regionKey = new RegionKey { Key = key, Region = region };
            _canRead = true;
            _canWrite = true;
            _canSeek = true;
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

        private void FlushWrite(bool calledFromFinalizer)
        {
            this.WriteCore(this._segment, 0, this._writePosition);
            this._writePosition = 0;
        }
        
        private void FlushRead()
        { }

        private void FlushInternalBuffer()
        {
            if (this._writePosition > 0)
                this.FlushWrite(false);
            else
                if (this._readPosition >= this._readLength || !this.CanSeek)
                    return;
            this.FlushRead();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        public override long Length
        {
            get 
            {
                SegmentStreamHeader header = GetHeader();
                long fileSize = header.Length;
                if (_writePosition > 0 && _position + _writePosition > fileSize)
                    fileSize = _writePosition + _position;
                return fileSize;
            }
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <returns></returns>
        private SegmentStreamHeader GetHeader()
        {
            return _cache.Get<SegmentStreamHeader>(Key, () => new SegmentStreamHeader(_segmentSize), _regionKey.Region);
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Reads the specified array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int Read(byte[] array, int offset, int count)
        {
            Assert.IsNotNull(array, "Array parameter is null");
            Assert.IsFalse(offset < 0, "Offset parameter is less than zero.");
            Assert.IsFalse(count < 0, "Count parameter is less than zero.");
            Assert.IsFalse(array.Length - offset < count, "Invalid offset and length, read would exceed bounds of array.");

            bool moreBytesToRead = false;
            int byteCount = _readLength - _readPosition;

            if (byteCount == 0)
            {
                if (!CanRead)
                    throw new NotSupportedException("Unable to read while the stream is disposing.");
                if (_writePosition > 0)
                    FlushWrite(false);
                if (!CanSeek || count >= _segmentSize)
                {
                    int bytesRead = ReadCore(array, 0, count);
                    this._readPosition = 0;
                    this._readLength = 0;
                    return bytesRead;
                }
                else 
                {
                    if (_segment == null)
                        _segment = new byte[_segmentSize];
                    byteCount = ReadCore(_segment, 0, _segmentSize);
                    if (byteCount == 0)
                        return 0;
                    moreBytesToRead = byteCount < _segmentSize;
                    _readPosition = 0;
                    _readLength = byteCount;
                }
            }

            if (byteCount > count)
            {
                byteCount = count;
            }

            Array.Copy(_segment, _readPosition, array, offset, byteCount);

            _readPosition += byteCount;
            if (byteCount < count && !moreBytesToRead)
            {
                int bytesRead = ReadCore(array, offset + byteCount, count - byteCount);
                byteCount += bytesRead;
                _readPosition = 0;
                _readLength = 0;
            }
            return byteCount;
        }

        /// <summary>
        /// Reads the core.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        private int ReadCore(byte[] array, int offset, int count)
        {
            int startSegmentIndex = SegmentUtility.GetSegmentIndex(_position, _segmentSize);
            int endSegmentIndex = SegmentUtility.GetSegmentIndex(_position + count, _segmentSize);

            int bytesRead = 0;

            // loop through segments
            for (int segmentIndex = startSegmentIndex; segmentIndex <= endSegmentIndex; segmentIndex++)
            {
                string segmentKey = SegmentUtility.GenerateSegmentKey(segmentIndex, _regionKey.Key);
                var segmentPosition = SegmentUtility.GetPositionInSegment(bytesRead + _position, _segmentSize);

                // calculate the byteCount
                var byteCount = _segmentSize - segmentPosition;
                if (count - bytesRead < byteCount)
                    byteCount = count - bytesRead;

                // are there bytes to read?
                if (byteCount > 0)
                {
                    // fetch the cache segment from cache
                    var segment = _cache.Get(segmentKey, _regionKey.Region) as byte[];
                    if (segment == null)
                        throw new InvalidOperationException(string.Format("Unable to read segment {0} from the Cache.", segmentKey));

                    // copy the segment from cache onto the array
                    System.Array.Copy(segment, segmentPosition, array, offset+bytesRead, byteCount);
                    bytesRead += byteCount;
                }
            }
            return bytesRead;
        }

        /// <summary>
        /// Reads a byte from the stream and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <returns>
        /// The unsigned byte cast to an Int32, or -1 if at the end of the stream.
        /// </returns>
        public override int ReadByte()
        {
            if (_readLength == 0 && !this.CanRead)
                throw new NotSupportedException("Read not supported while disposing.");
            if (_readPosition == _readLength)
            {
                if (_writePosition > 0)
                    FlushWrite(false);
                if (_segment == null)
                    _segment = new byte[_segmentSize];
                _readLength = ReadCore(_segment, 0, _segmentSize);
                _readPosition = 0;
            }
            if (_readPosition == _readLength)
                return -1;
            int byteRead = (int)_segment[_readPosition];
            ++_readPosition;
            return byteRead;
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
                throw new ArgumentException("Invalid SeekOrigin");
            if (!CanSeek)
                throw new NotSupportedException("Seek is disabled while disposing");
            if (_writePosition > 0)
                FlushWrite(false);
            else if (origin == SeekOrigin.Current)
                offset -= (long)(_readLength - _readPosition);
            long relativeOffset = _position + (_readPosition - _readLength);
            long position = SeekCore(offset, origin);

            if (_readLength > 0)
            {
                if (relativeOffset == position)
                {
                    if (_readPosition > 0)
                    {
                        Array.Copy(_segment, _readPosition, _segment, 0, _readLength - _readPosition);
                        _readLength -= _readPosition;
                        _readPosition = 0;
                    }
                    if (_readLength > 0)
                        SeekCore(_readLength, SeekOrigin.Current);
                }
                else if (relativeOffset - _readPosition < position && position < relativeOffset + _readLength - _readPosition)
                {
                    int seekBytes = (int)(position - relativeOffset);
                    Array.Copy(_segment, _readPosition + seekBytes, _segment, 0, _readLength - (_readPosition + seekBytes));
                    _readLength -= _readPosition + seekBytes;
                    _readPosition = 0;
                    if (_readLength > 0)
                        SeekCore(_readLength, SeekOrigin.Current);
                }
                else 
                {
                    _readLength = 0;
                    _readPosition = 0;
                }
            }
            return position;
        }

        /// <summary>
        /// Seeks the core.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private long SeekCore(long offset, SeekOrigin origin)
        {
            var length = GetHeader().Length;
            var position = offset;
            if(origin == SeekOrigin.End)
                position = length - offset;
            if(origin == SeekOrigin.Current)
                position = _position + offset;
            _position = position;
            return position;
        }

        /// <summary>
        /// When overridden in a derived class, sets the length of the current stream.
        /// </summary>
        /// <param name="value">The desired length of the current stream in bytes.</param>
        public override void SetLength(long value)
        {
            Assert.IsTrue(value >= 0L, "Value must be greater than or equal to zero.");
            Assert.IsTrue(CanSeek, "Seek is not supported while disposing.");
            Assert.IsTrue(CanWrite, "Write is not supported while disposing.");

            // if there is a partial write, flush the write
            if (_writePosition > 0)
                FlushWrite(false);
            // if there is a partial read, flush the read
            else if (_readPosition < _readLength)
                FlushRead();
            this._readPosition = 0;
            this._readLength = 0;

            this.SetLengthCore(value);
        }
        
        private void SetLengthCore(long value)
        {
        }

        /// <summary>
        /// Writes the specified array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <exception cref="System.InvalidOperationException">Write is not supported while disposing.</exception>
        public override void Write(byte[] array, int offset, int count)
        {
            Assert.IsNotNull(array, "Array cannot be null.");
            Assert.IsTrue(offset >= 0, "Offset cannot be less than zero.");
            Assert.IsTrue(count >= 0, "Count cannot be less than zero.");
            Assert.IsFalse(array.Length - offset < count, "Invalid array size. Count + Offset cannot be greater than the array length.");

            // write is starting the beginning of a segment
            if (_writePosition == 0)
            {
                if (!this.CanWrite)
                    throw new InvalidOperationException("Write is not supported while disposing.");
                if (this._readPosition < this._readLength)
                    this.FlushRead();
                this._readLength = 0;
                this._readPosition = 0;
            }

            // write is starting in the middle of a segment
            if (_writePosition > 0)
            {
                // get the number of bytes we can write
                int byteCount = _segmentSize - _writePosition;

                // if the byte count is greater than the total count
                // we will write the total count
                if (byteCount > count)
                    byteCount = count;

                // copy the input bytes to our buffer
                System.Array.Copy(array, offset, _segment, _writePosition, byteCount);
                _writePosition += byteCount;

                // we have written all of the bytes
                if (count == byteCount)
                    return;

                // adjust the offset and count to match the partial segmenet we
                // sent to the buffer
                offset += byteCount;
                count -= byteCount;

                this.WriteCore(_segment, 0, _writePosition);
                _writePosition = 0;
            }

            if (count >= _segmentSize)
            {
                WriteCore(array, offset, count);
            }
            else 
            {
                // is the count zero? return
                if (count == 0)
                    return;

                // allocate the buffer if it does not exist
                if (_segment == null)
                    _segment = new byte[_segmentSize];

                // move the bytes from the array into the buffer
                System.Array.Copy(array, offset, _segment, _writePosition, count);

                // update the write position
                _writePosition = count;
            }
        }

        /// <summary>
        /// Writes a byte to the current position in the stream and advances the position within the stream by one byte.
        /// </summary>
        /// <param name="value">The byte to write to the stream.</param>
        public override void WriteByte(byte value)
        {
            if (_writePosition == 0)
            {
                if (!this.CanWrite)
                    throw new InvalidOperationException("Write is not supported while disposing.");
                if (_readPosition < _readLength)
                    FlushRead();
                _readPosition = 0;
                _readLength = 0;
                if (_segment == null)
                    _segment = new byte[_segmentSize];                                    
            }
            if (_writePosition == _segmentSize)
            {
                FlushWrite(false);
            }
            _segment[_writePosition] = value;
            _writePosition++;
        }

        /// <summary>
        /// Takes an array an splits the array into segments writing each to the cache
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        private void WriteCore(byte[] array, int offset, int count)
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
            var header = GetHeader();
            if (_position > header.Length)
            {
                header.Length = _position;
                _cache.Set(Key, header, Region);
            }
        }
    }
}
