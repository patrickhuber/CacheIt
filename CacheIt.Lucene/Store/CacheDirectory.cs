using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Creates a stream based directory
    /// </summary>
    public class CacheDirectory : Directory
    {
        /// <summary>
        /// Creates a new, empty file in the directory with the given name.
        /// Returns a stream writing this file.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IndexOutput CreateOutput(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes an existing file in the directory.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void DeleteFile(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns true iff a file with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool FileExists(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the length of a file in the directory.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long FileLength(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the time the named file was last modified.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long FileModified(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an array of strings, one for each file in the directory.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string[] ListAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a stream reading an existing file.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override IndexInput OpenInput(string name)
        {
            this.EnsureOpen();
            throw new NotImplementedException();
        }

        /// <summary>
        /// Set the modified time of an existing file to now.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void TouchFile(string name)
        {
            throw new NotImplementedException();
        }
    }
}
