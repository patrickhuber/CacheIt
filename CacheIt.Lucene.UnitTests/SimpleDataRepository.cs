using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lucene.Net.Search;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Store;
using System.Runtime.Caching;

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
  
        public SimpleData Get(int id)
        {
            return Items.FirstOrDefault(x => x.Id.Equals(id));
        }

        public List<SimpleData> GetAll()
        {
            return Items;        
        }

        static SimpleDataRepository()
        {
            var objectCache = new MemoryCache("Default");
            var directory = new CacheIt.Lucene.Store.CacheDirectory(objectCache, "/path/to/directory");

        }

        private static void AddUpdateLuceneIndex(IEnumerable<SimpleData> simpleDataList)
        { 
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            using(var writer = new IndexWriter())
        }

        private static void AddToLuceneIndex(SimpleData simpleData, IndexWriter writer)
        {
            // remove older index entries
            var searchQuery = new TermQuery(new Term("Id", simpleData.Id.ToString()));
            writer.DeleteDocuments(searchQuery);

            // add new index entry
            var doc = new Document();

            // add lucene fields mapped to our object
            doc.Add(new Field("Id", simpleData.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Name", simpleData.Name, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Description", simpleData.Description, Field.Store.YES, Field.Index.ANALYZED));

            // add entry to the index
            writer.AddDocument(doc);
        }
    }
}
