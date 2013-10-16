﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using System.IO;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Output Stream for writing a file to a cache directory
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
            get { return stream.Position; }
        }

        /// <summary>
        /// The number of bytes in the file.
        /// </summary>
        /// <returns></returns>
        public override long Length()
        {
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
            return (byte)stream.ReadByte();
        }

        /// <summary>
        /// Reads a specified number of bytes into an array at the specified offset.
        /// </summary>
        /// <param name="b">the array to read bytes into</param>
        /// <param name="offset">the offset in the array to start storing bytes</param>
        /// <param name="len">the number of bytes to read</param>
        /// <seealso cref="M:Lucene.Net.Store.IndexOutput.WriteBytes(System.Byte[],System.Int32)">
        ///   </seealso>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void ReadBytes(byte[] b, int offset, int len)
        {
            stream.Read(b, offset, len);
        }

        /// <summary>
        /// Sets current position in this file, where the next read will occur.
        /// </summary>
        /// <param name="pos"></param>
        /// <seealso cref="P:Lucene.Net.Store.IndexInput.FilePointer">
        ///   </seealso>
        public override void Seek(long pos)
        {
            stream.Seek(pos, SeekOrigin.Begin);
        }
    }
}
