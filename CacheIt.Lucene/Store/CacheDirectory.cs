using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Store;
using System.Runtime.Caching;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Creates a stream based directory
    /// </summary>
    public class CacheDirectory : Directory
    {
        private string directory;
        private ObjectCache objectCache;
        private ISet<string> files;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDirectory"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="cache">The cache.</param>
        public CacheDirectory(string directory, ObjectCache cache) : base()
        {            
            this.objectCache = cache;
            this.directory = directory;

            // lets try with string lists
            // if this turns out to be inefficent, it may be beneficial to add a 
            // implementation specific list to CacheBase but I would prefer to avoid this
            // apprach at all costs.
            this.files = objectCache.Get(directory, () => new HashSet<string>());
        }

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
            // get a copy of the list
            var files = this.objectCache.Get(directory);
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
