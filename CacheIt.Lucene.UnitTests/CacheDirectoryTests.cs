using CacheIt;
using CacheIt.Lucene.Store;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;

namespace CacheIt.Lucene.UnitTests
{
    [TestClass]
    public class CacheDirectoryTests
    {
        const string DIRECTORY = "test_directory";

        Directory directory;
        ObjectCache cache;

        [TestInitialize]
        public void Initialize_CacheDirectoryTests()
        {
            cache = new MemoryCache("TEST");
            directory = new CacheDirectory(cache, DIRECTORY);
        }

        [TestMethod]
        public void Test_CreateOutput_Creates_Output_Stream()
        {
            const string FILE = "myfile.txt";
            using (var output = directory.CreateOutput(FILE))
            { }
            Assert.IsTrue(cache.Get(
                string.Format(
                "{0}\\{1}", DIRECTORY, FILE)) != null);
        }

        [TestMethod]
        public void Test_DeleteFile_DeletesFile()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);

            directory.DeleteFile(FILE);
            Assert.IsTrue(cache.Get(
                string.Format(
                "{0}\\{1}", DIRECTORY, FILE)) == null);            
        }

        [TestMethod]
        public void Test_FileExists_Detects_File()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);

            Assert.IsTrue(directory.FileExists(FILE));
        }

        [TestMethod]
        public void Test_FileModified_Returns_Modified_Time()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);

            var oldModified = directory.FileModified(FILE);

            Assert.AreNotEqual(0, oldModified);
        }

        [TestMethod]
        public void Test_ListAll_Returns_All_Items()
        {
            const int FILES_LENGTH = 100;
            const string FILE_TEMPLATE = "myfile_{0}.txt";
            for (int i = 0; i < FILES_LENGTH; i++)
            {
                string fileName = string.Format(FILE_TEMPLATE, i);
                CreateFile(fileName);               
            }
            string[] files = directory.ListAll();
            Assert.AreEqual(FILES_LENGTH, files.Length);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void Test_OpenItem_Throws_FileNotFoundException()
        {
            const string FILE = "myfile.txt";
            using (var file = directory.OpenInput(FILE))
            { }
            Assert.Fail();
        }

        [TestMethod]
        public void Test_OpenInput_Creates_Input_Stream()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);
            using (var file = directory.OpenInput(FILE))
            {
                Assert.IsNotNull(file);
            }
        }

        [TestMethod]
        public void Test_TouchFile_Sets_Modified_Time()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);

            var oldModified = directory.FileModified(FILE);
            Thread.Sleep(1);
            directory.TouchFile(FILE);
            var newModified = directory.FileModified(FILE);

            Assert.IsTrue(newModified > oldModified);
        }

        private void CreateFile(string fileName)
        {
            using (var output = directory.CreateOutput(fileName))
            { }
        }

        [Ignore]
        public void Test_FileSystem_Files_Match_Cache_Files()
        {
            // create the cache index
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(25000));
            Document document = new Document();
            string text = "This is the text to be indexed.";
            document.Add(new Field("fieldname", text, Field.Store.YES, Field.Index.ANALYZED));
            indexWriter.AddDocument(document);
            indexWriter.Dispose();

            // create the filesystem index
            string fileSystemDirectory = @"C:\x\cacheit";
            if(System.IO.Directory.Exists(fileSystemDirectory))
                System.IO.Directory.Delete(fileSystemDirectory, true);
            var fsDirectory = FSDirectory.Open(fileSystemDirectory);
            indexWriter = new IndexWriter(fsDirectory, analyzer, true, new IndexWriter.MaxFieldLength(25000));
            document = new Document();
            document.Add(new Field("fieldname", text, Field.Store.YES, Field.Index.ANALYZED));
            indexWriter.AddDocument(document);
            indexWriter.Dispose();

            var cacheFiles = directory.ListAll().OrderBy(x=>x).ToArray();
            var fileSystemFiles = fsDirectory.ListAll().OrderBy(x=>x).ToArray();

            // make sure the file counts match
            Assert.AreEqual(cacheFiles.Length, fileSystemFiles.Length);

            for (int i = 0; i < cacheFiles.Length; i++)
            {
                Assert.AreEqual(cacheFiles[i], fileSystemFiles[i]);
                string name = cacheFiles[i];
                var cacheFileStream = directory.OpenInput(name);
                var fsFileStream = fsDirectory.OpenInput(name);
                Assert.AreEqual(cacheFileStream.Length(), fsFileStream.Length());
                int length = Convert.ToInt32(fsFileStream.Length());
                byte[] cacheBuffer = new byte[length];
                byte[] fsBuffer = new byte[length];

                cacheFileStream.ReadBytes(cacheBuffer, 0, cacheBuffer.Length);
                fsFileStream.ReadBytes(fsBuffer, 0, fsBuffer.Length);

                for (int b = 0; b < cacheBuffer.Length; b++)
                {
                    Assert.AreEqual(cacheBuffer[b], fsBuffer[b], 
                        string.Format("File {0} byte {1} expected {2} actual {3}", name, b, fsBuffer[b], cacheBuffer[b]));
                }
            }
        }

        [TestMethod]
        public void Test_Create_And_Read_Index()
        {
            // create the index
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(25000));
            Document document = new Document();
            string text = "This is the text to be indexed.";            
            document.Add(new Field("fieldname", text, Field.Store.YES, Field.Index.ANALYZED));
            indexWriter.AddDocument(document);
            indexWriter.Dispose();

            // search the index
            IndexSearcher indexSearcher = new IndexSearcher(directory, true);
            QueryParser parser = new QueryParser(Version.LUCENE_30, "fieldname", analyzer);
            Query query = parser.Parse("text");
            ScoreDoc[] hits = indexSearcher.Search(query, null, 1000).ScoreDocs;
            Assert.AreEqual(1, hits.Length);
                        
            // iterate through the results
            string expectedText = string.Copy(text);
            for(int i=0;i<hits.Length;i++)
            {
                Document hitDocument = indexSearcher.Doc(hits[i].Doc);
                Assert.AreEqual(hitDocument.Get("fieldname"), expectedText);
            }
            indexSearcher.Dispose();
            directory.Dispose();
        }
    }
}
