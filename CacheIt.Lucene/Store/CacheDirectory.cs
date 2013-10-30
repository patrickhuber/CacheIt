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

        private void AddFileToList(string fileName)
        {
            var set = this.objectCache.Get(directory) as ISet<string>;
            if (set.Contains(fileName))
                set.Add(fileName);
            this.objectCache.Set(directory, set, region);
        }

        private void RemoveFileFromList(string fileName)
        {
            var set = this.objectCache.Get(directory) as ISet<string>;
            if (set.Contains(fileName))
                set.Remove(fileName);
            this.objectCache.Set(directory, set, region);
        }

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
            var hashSet = objectCache.Get(directory, () => new HashSet<string>());
            files = new PersistentSetAdapter<string>(hashSet, objectCache, directory, region);

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
            CacheFile cacheFile = this.objectCache.Get(name, region) as CacheFile;

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
                    objectCache.Remove(dataIdentifier);
                }

                // remove the cache file last in case of exception
                this.objectCache.Remove(fileKey, region);
            }
            
            // update the filesystem
            this.RemoveFileFromList(name);
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
            return this.objectCache.Contains(GenerateFileKey(name), region);
        }

        /// <summary>
        /// Returns the length of a file in the directory.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>the length of the file</returns>        
        public override long FileLength(string name)
        {
            var fileKey = GenerateFileKey(name);
            var cacheFile = this.objectCache.Get(fileKey, region) as CacheFile;
            
            if (cacheFile == null)
                throw new FileNotFoundException(
                    string.Format(
                        "Unable to find file {0} in directory {1}",
                        name,
                        directory));

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
            var fileKey = GenerateFileKey(name);
            var cacheFile = this.objectCache.Get(fileKey, region);
            if (cacheFile == null)
                throw new FileNotFoundException(
                    string.Format(
                        "Unable to find file {0} in directory {1}",
                        name,
                        directory));
            return (cacheFile as CacheFile).LastModified;
        }

        /// <summary>
        /// Returns an array of strings, one for each file in the directory.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override string[] ListAll()
        {
            // get a copy of the list
            var files = this.objectCache.Get(directory, region) as HashSet<string>;
            if (files == null)
                throw new IOException(string.Format("Unable to enumerate files in directory {0}. File list is null.", directory));
            return files.ToArray();

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
            return new CacheInputStream(
                new ChunkStream(this.objectCache, GenerateFileKey(name), region));
        }

        /// <summary>
        /// Set the modified time of an existing file to now.
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="System.NotImplementedException"></exception>
        public override void TouchFile(string name)
        {
            var fileKey = GenerateFileKey(name);
            var instance = this.objectCache.Get(fileKey, region);
            if (instance == null)
                throw new FileNotFoundException(
                    string.Format(
                        "Unable to find file {0} in directory {1}",
                        name,
                        directory));
            var cacheFile = (instance as CacheFile);
            
            // set last modifed to the value used by Lucene RAM Directory
            cacheFile.LastModified = DateTimeOffset.UtcNow.Ticks / 10000L;

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
    }
}
