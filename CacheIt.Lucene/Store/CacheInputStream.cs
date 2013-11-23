using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using System.IO;
using System.Diagnostics;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Output Stream for reading a file from a cache directory
    /// </summary>
    public class CacheInputStream : IndexInput
    {
        private Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheInputStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public CacheInputStream(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("CacheInputStream.Dispose(disposing={0})", disposing);
            stream.Dispose();
        }

        /// <summary>
        /// Returns the current position in this file, where the next read will
        /// occur.
        /// </summary>
        /// <seealso cref="M:Lucene.Net.Store.IndexInput.Seek(System.Int64)">
        ///   </seealso>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long FilePointer
        {
            get
            {
                Debug.WriteLine("CacheInputStream.FilePointer");
                return stream.Position;
            }
        }

        /// <summary>
        /// The number of bytes in the file.
        /// </summary>
        /// <returns></returns>
        public override long Length()
        {
            Debug.WriteLine("CacheInputStream.Length");
            return stream.Length;
        }

        /// <summary>
        /// Reads and returns a single byte.
        /// </summary>
        /// <returns></returns>
        /// <seealso cref="M:Lucene.Net.Store.IndexOutput.WriteByte(System.Byte)">
        ///   </seealso>
        public override byte ReadByte()
        {
            byte value = (byte)stream.ReadByte();
            Debug.WriteLine("CacheInputStream.ReadByte() r={0}", value);
            return value;
        }

        /// <summary>
        /// Reads a specified number of bytes into an array at the specified offset.
        /// </summary>
        /// <param name="b">the array to read bytes into</param>
        /// <param name="offset">the offset in the array to start storing bytes</param>
        /// <param name="length">the number of bytes to read</param>
        /// <seealso cref="M:Lucene.Net.Store.IndexOutput.WriteBytes(System.Byte[],System.Int32)">
        ///   </seealso>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void ReadBytes(byte[] b, int offset, int length)
        {
            stream.Read(b, offset, length);
            Debug.WriteLine("CacheInputStream.ReadBytes(b={0}, offset={1}, len={2})", BitConverter.ToString(b, offset, length), offset, length);            
        }

        /// <summary>
        /// Sets current position in this file, where the next read will occur.
        /// </summary>
        /// <param name="pos"></param>
        /// <seealso cref="P:Lucene.Net.Store.IndexInput.FilePointer">
        ///   </seealso>
        public override void Seek(long pos)
        {
            Debug.WriteLine("CacheInputStream.Seek(pos={0})", pos);
            stream.Seek(pos, SeekOrigin.Begin);
        }
    }
}
