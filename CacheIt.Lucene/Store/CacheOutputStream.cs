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
    /// Creates an output stream for writing to a cache stream.
    /// </summary>
    public class CacheOutputStream : IndexOutput
    {
        private Stream stream;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheOutputStream"/> class.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public CacheOutputStream(Stream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            Debug.WriteLine("CacheOutputStream.Dispose(disposing={0})", disposing);
            stream.Dispose();
        }

        /// <summary>
        /// Returns the current position in this file, where the next write will
        /// occur.
        /// </summary>
        /// <seealso cref="M:Lucene.Net.Store.IndexOutput.Seek(System.Int64)">
        ///   </seealso>
        public override long FilePointer
        {
            get 
            { 
                Debug.WriteLine("CacheOutputStream.FilePointer");
                return stream.Position; 
            }
        }

        /// <summary>
        /// Forces any buffered output to be written.
        /// </summary>
        public override void Flush()
        {
            Debug.WriteLine("CacheOutputStream.Flush()");
            stream.Flush();
        }

        /// <summary>
        /// The number of bytes in the file.
        /// </summary>
        public override long Length
        {
            get
            {
                Debug.WriteLine("CacheOutputStream.Length");
                return stream.Length;
            }
        }

        /// <summary>
        /// Sets current position in this file, where the next write will occur.
        /// </summary>
        /// <param name="pos"></param>
        /// <seealso cref="P:Lucene.Net.Store.IndexOutput.FilePointer">
        ///   </seealso>
        public override void Seek(long pos)
        {
            Debug.WriteLine("CacheOutputStream.Seek(pos={0})", pos);
            stream.Seek(pos, SeekOrigin.Begin);
        }

        /// <summary>
        /// Writes a single byte.
        /// </summary>
        /// <param name="b"></param>
        /// <seealso cref="M:Lucene.Net.Store.IndexInput.ReadByte">
        ///   </seealso>
        public override void WriteByte(byte b)
        {
            Debug.WriteLine("CacheOutputStream.WriteByte(b={0})", b);
            stream.WriteByte(b);
        }

        /// <summary>
        /// Writes an array of bytes.
        /// </summary>
        /// <param name="b">the bytes to write</param>
        /// <param name="offset">the offset in the byte array</param>
        /// <param name="length">the number of bytes to write</param>
        /// <seealso cref="M:Lucene.Net.Store.IndexInput.ReadBytes(System.Byte[],System.Int32,System.Int32)">
        ///   </seealso>
        public override void WriteBytes(byte[] b, int offset, int length)
        {
            Debug.WriteLine("CacheOutputStream.WriteBytes(b={0}, offset={1}, length={2})", BitConverter.ToString(b, offset, length), offset, length);
            stream.Write(b, offset, length);
        }
    }
}
