using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Lucene.Net.Store;
using CacheIt.Lucene.Store;
using Lucene.Net.Documents;
using System;
using Lucene.Net.Search;

namespace CacheIt.Lucene.UnitTests
{
    /// <summary>
    /// <see cref="http://www.codeproject.com/Articles/320219/Lucene-Net-ultra-fast-search-for-MVC-or-WebForms#Step1"/>
    /// </summary>
    public class SimpleDataRepository
    {
        public static List<SimpleData> Items = new List<SimpleData> {
            new SimpleData {Id = 1, Name = "Belgrad", Description = "City in Serbia"},
            new SimpleData {Id = 2, Name = "Moscow", Description = "City in Russia"},
            new SimpleData {Id = 3, Name = "Chicago", Description = "City in USA"},
            new SimpleData {Id = 4, Name = "Mumbai", Description = "City in India"},
            new SimpleData {Id = 5, Name = "Hong-Kong", Description = "City in Hong-Kong"},
        };

        private static SimpleDataIndex simpleDataIndex;

        static SimpleDataRepository()
        {
            const bool USE_FILESYSTEM = false;
            const string DIRECTORY = "C:\\x\\cacheit";

            var objectCache = new MemoryCache("{7D8DAD94-A0EF-4168-ACB5-2574DC000F26}");
            Directory directory = null;

            if (USE_FILESYSTEM)
            {
                if (System.IO.Directory.Exists(DIRECTORY))
                    System.IO.Directory.Delete(DIRECTORY, true);
                System.IO.Directory.CreateDirectory(DIRECTORY);
                directory = FSDirectory.Open(DIRECTORY);
            }
            else 
            { 
                directory = new CacheDirectory(objectCache, DIRECTORY);
            }            
            
            simpleDataIndex = new SimpleDataIndex(Items, directory);
        }

        public SimpleDataRepository()
        {
        }

        /// <summary>
        /// Gets the specified unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        /// <returns></returns>
        public SimpleData Get(int id)
        {
            return Items.FirstOrDefault(x => x.Id.Equals(id));
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SimpleData> GetAll()
        {
            return Items;        
        }

        /// <summary>
        /// Searches the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public IEnumerable<SimpleData> Search(string query)
        {
            return simpleDataIndex.Search(query);
        }       
    }
}
