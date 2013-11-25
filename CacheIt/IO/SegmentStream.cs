using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace CacheIt.IO
{
    public class SegmentStream : Stream
    {
        private BufferedStream bufferedStream;


        public const int DefaultSegmentSize = 1024;
        public const string DefaultRegion = null;

        public SegmentStream(ObjectCache objectCache, string key)
            : this(objectCache, key, DefaultSegmentSize)
        { 
        }

        public SegmentStream(ObjectCache objectCache, string key, int segmentSize, string region = DefaultRegion)
        {
            bufferedStream = new BufferedStream(
                new InternalSegmentStream(objectCache, key, segmentSize, region));
        }

        public override bool CanRead
        {
            get { return bufferedStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return bufferedStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return bufferedStream.CanWrite; }
        }

        public override void Flush()
        {
            bufferedStream.Flush();
        }

        public override long Length
        {
            get { return bufferedStream.Length; }
        }

        public override long Position
        {
            get
            {
                return bufferedStream.Position;
            }
            set
            {
                bufferedStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return bufferedStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return bufferedStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            bufferedStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            bufferedStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.Stream" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                bufferedStream.Flush();
        }
    }
}
