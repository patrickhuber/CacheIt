using CacheIt.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.IO
{
    public class BufferedStream : Stream
    {
        private ObjectCache _cache;
        private RegionKey _regionKey;

        private bool _canRead;
        private bool _canSeek;
        private bool _canWrite;
        private readonly int _bufferSize;
        private byte[] _buffer;
        private long _bufferPosition;
        private int _readPosition;
        private int _readLength;
        // the position within the current buffer of a write
        private int _writePosition;
        private long _position;

        public BufferedStream(ObjectCache objectCache, string key, string region, int bufferSize)
        {
            _buffer = new byte[bufferSize];
            _bufferSize = bufferSize;
            _cache = objectCache;
            _regionKey = new RegionKey { Key = key, Region = region };
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
            this.WriteCore(this._buffer, 0, this._writePosition);
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

        public override long Length
        {
            get { throw new NotImplementedException(); }
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

        public override int Read(byte[] array, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
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
            Assert.IsTrue(array.Length - offset >= count, "Invalid array size. Count + Offset cannot be greater than the array length.");

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
                int byteCount = _bufferSize - _writePosition;

                // if the byte count is greater than the total count
                // we will write the total count
                if (byteCount > count)
                    byteCount = count;

                // copy the input bytes to our buffer
                Array.Copy(array, offset, _buffer, _writePosition, byteCount);
                _writePosition += byteCount;

                // we have written all of the bytes
                if (count == byteCount)
                    return;

                // adjust the offset and count to match the partial segmenet we
                // sent to the buffer
                offset += byteCount;
                count -= byteCount;

                this.WriteCore(_buffer, 0, _writePosition);
                _writePosition = 0;
            }

            if (count >= _bufferSize)
            {
                WriteCore(array, offset, count);
            }
            else 
            {
                // is the count zero? return
                if (count == 0)
                    return;

                // allocate the buffer if it does not exist
                if (_buffer == null)
                    _buffer = new byte[_bufferSize];

                // move the bytes from the array into the buffer
                Array.Copy(array, offset, _buffer, _writePosition, count);

                // update the write position
                _writePosition = count;
            }
        }

        /// <summary>
        /// Writes the core.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        private void WriteCore(byte[] array, int offset, int count)
        {
            int segmentIndex = GetSegmentIndex(_position, _bufferSize);
            string segmentKey = GenerateSegmentKey(segmentIndex, _regionKey.Key);
            var buffer = _cache.Get<byte[]>(segmentKey, () => new byte[_bufferSize], _regionKey.Region);
            Array.Copy(array, offset, buffer, offset, count);
            _cache.Set(segmentKey, buffer, _regionKey.Region);
            _position += count;
        }

        /// <summary>
        /// Gets the index of the segment.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="bufferSize">Size of the buffer.</param>
        /// <returns></returns>
        private static int GetSegmentIndex(long position, int bufferSize)
        {
            return Convert.ToInt32(position / (long)bufferSize);
        }

        /// <summary>
        /// Generates the segment key.
        /// </summary>
        /// <param name="segmentIndex">Index of the segment.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private static string GenerateSegmentKey(int segmentIndex, string key)
        {
            return string.Format("{0}_{1}", key, segmentIndex);
        }
    }
}
