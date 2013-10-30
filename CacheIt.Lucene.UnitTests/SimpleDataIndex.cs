using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

namespace CacheIt.Lucene.UnitTests
{
    public class SimpleDataIndex
    {        
        private Directory directory;
        
        public SimpleDataIndex(List<SimpleData> items, Directory directory)
        {
            this.directory = directory;
            AddUpdateLuceneIndex(items);
        }

        public IEnumerable<SimpleData> Search(string searchQuery)
        {
            // validation
            if (string.IsNullOrEmpty(
                searchQuery
                    .Replace("*", "")
                    .Replace("?", "")))
                return new List<SimpleData>();

            using (var searcher = new IndexSearcher(directory, false))
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_30);
                var parser = new MultiFieldQueryParser(Version.LUCENE_30, new[] { "Id", "Name", "Description" }, analyzer);
                var query = parseQuery(searchQuery, parser);
                var hits = searcher.Search(query, null, 1000, Sort.RELEVANCE).ScoreDocs;
                var results = Map(hits, searcher);
                analyzer.Close();
                searcher.Dispose();
                return results;
            }
        }

        /// <summary>
        /// Maps the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        private SimpleData Map(Document document)
        {
            return new SimpleData
            {
                Id = Convert.ToInt32(document.Get("Id")),
                Name = document.Get("Name"),
                Description = document.Get("Description")
            };
        }

        private IEnumerable<SimpleData> Map(IEnumerable<Document> hits)
        {
            return hits.Select(Map).ToList();
        }

        private IEnumerable<SimpleData> Map(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => Map(searcher.Doc(hit.Doc))).ToList();
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        } 

        private void AddUpdateLuceneIndex(IEnumerable<SimpleData> simpleDataList)
        {
            using (var analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    foreach (var simpleData in simpleDataList)
                    {
                        AddToLuceneIndex(simpleData, writer);
                    }
                }
            }
        }

        private void AddToLuceneIndex(SimpleData simpleData, IndexWriter writer)
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
