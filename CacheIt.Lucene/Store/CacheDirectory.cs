using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;
using Lucene.Net.Store;
using CacheIt.IO;
using System.IO;
using Directory = Lucene.Net.Store.Directory;
using CacheIt.Collections;
using System.Diagnostics;

namespace CacheIt.Lucene.Store
{
    /// <summary>
    /// Defines a cache dictionary that contains files
    /// <remarks>The cache directory is a container for files. The container holds a list of file names that are used to access CacheFile objects.</remarks>    
    /// </summary>
    public class CacheDirectory : Directory
    {
        private string directory;
        private string region;
        private ObjectCache objectCache;
        private ISet<string> files;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDirectory"/> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="directory">The directory.</param>
        public CacheDirectory(ObjectCache cache, string directory)
            : this(cache, directory, null)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDirectory"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="cache">The cache.</param>
        public CacheDirectory(ObjectCache cache, string directory, string region) : base()
        {            
            this.objectCache = cache;
            this.directory = directory;
            this.region = region;

            // lets try with string lists
            // if this turns out to be inefficent, it may be beneficial to add a 
            // implementation specific list to CacheBase but I would prefer to avoid this
            // apprach at all costs.
            var hashSet = objectCache.Get(directory, () => new HashSet<string>(), region);
            files = new PersistentSet<string>(hashSet, objectCache, directory, region);

            base.SetLockFactory(new CacheLockFactory(objectCache, string.Format("{0}_{1}", directory, "lock")));
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
            this.EnsureOpen();

            // create the file key
            var fileKey = GenerateFileKey(name);
            
            // attempt to get an existing object
            CacheFile cacheFile = this.objectCache.Get(fileKey, region) as CacheFile;

            // if the object exists, completely remove the item
            if (cacheFile != null)
            {
                DeleteFile(name);
            }

            // create a new instance of the cache file to be stored
            cacheFile = new CacheFile
            {
                Identifier = Guid.NewGuid().ToString(),
                LastModified = DateTimeOffset.UtcNow.Ticks / 10000L
            };            

            // set the cache file info under the file key
            this.objectCache.Set(fileKey, cacheFile, region);            

            // update the filesystem
            this.files.Add(name);

            Trace.WriteLine(string.Format("File {0} created", name));

            // return the stream to the new file
            return new CacheOutputStream(
                new ChunkStream(this.objectCache, cacheFile.Identifier, region));
        }

        /// <summary>
        /// Removes an existing file in the directory.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void DeleteFile(string name)
        {
            // get the file key and fetch the file info from the cache
            var fileKey = GenerateFileKey(name);
            var cacheFile = this.objectCache.Get(fileKey, region);

            // not be null to continue
            if (cacheFile != null)
            {
                // get the data identifier and remove the data
                string dataIdentifier = (cacheFile as CacheFile).Identifier;
                if (!string.IsNullOrWhiteSpace(dataIdentifier))
                {
                    // remove the data
                    using (var stream = new ChunkStream(this.objectCache, dataIdentifier, region))
                    {
                        stream.SetLength(0);
                    }
                    // remove the stream info
                    objectCache.Remove(dataIdentifier, region);
                }

                // remove the cache file last in case of exception
                this.objectCache.Remove(fileKey, region);
            }
            
            // update the filesystem
            this.files.Remove(name);
            
            Trace.WriteLine(string.Format("File {0} deleted", name));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected override void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Returns true iff a file with the given name exists.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool FileExists(string name)
        {            
            bool result = this.objectCache.Contains(GenerateFileKey(name), region);
            Trace.WriteLine(string.Format("File {0} does {1}exist", name, result ? string.Empty : "not "));
            return result;
        }

        /// <summary>
        /// Returns the length of a file in the directory.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the length of the file</returns>        
        public override long FileLength(string name)
        {
            // fetch the cache file
            var cacheFile = GetCacheFile(name);

            // use the cache file identifier to return a stream that contains the length
            // of the file
            using(var fileStream = new ChunkStream(this.objectCache, cacheFile.Identifier, region))
            {
                return fileStream.Length;
            }
        }

        /// <summary>
        /// Returns the time the named file was last modified.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the last modified date of the file</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override long FileModified(string name)
        {
            var cacheFile = GetCacheFile(name);
            return (cacheFile as CacheFile).LastModified;
        }

        /// <summary>
        /// Returns an array of strings, one for each file in the directory.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string[] ListAll()
        {
            // check if the file list is null
            if (files == null)
                throw new IOException(string.Format("Unable to enumerate files in directory {0}. File list is null.", directory));

            // get a copy of the list
            var copyOfFiles = new HashSet<string>(this.files);
            
            Trace.WriteLine(string.Format("ListAll called with {0} files.", copyOfFiles.Count));

            return copyOfFiles.ToArray();
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

            // fetch the cache file from the store
            var cacheFile = GetCacheFile(name);
            
            // create a chunk stream with the cache file identifier
            var chunkStream = new ChunkStream(this.objectCache, cacheFile.Identifier, region);

            Trace.WriteLine(string.Format("{0} open for read", name));

            // return a new cache input stream from the chunk stream.
            return new CacheInputStream(chunkStream);
        }

        /// <summary>
        /// Set the modified time of an existing file to now.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void TouchFile(string name)
        {            
            var cacheFile = GetCacheFile(name);
            
            // set last modifed to the value used by Lucene RAM Directory
            cacheFile.LastModified = DateTimeOffset.UtcNow.Ticks / 10000L;

            // recalculate the file Key
            var fileKey = GenerateFileKey(name);

            // save the object back to the cache
            this.objectCache.Set(fileKey, cacheFile, region);
        }

        /// <summary>
        /// Generates the file key.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        protected virtual string GenerateFileKey(string name)
        {
            return System.IO.Path.Combine(directory, name);
        }

        /// <summary>
        /// Gets the cache file.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        protected virtual CacheFile GetCacheFile(string name)
        {
            string fileKey = GenerateFileKey(name);
            var cacheFile = this.objectCache.Get(fileKey, region) as CacheFile;
            if (cacheFile == null)
                throw new FileNotFoundException(
                    string.Format(
                        "Unable to find file {0} in directory {1}",
                        name,
                        directory));
            return cacheFile;
        }
    }
}
