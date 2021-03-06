﻿using CacheIt;
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

namespace CacheIt.Lucene.Tests.Unit
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
        public void Test_CacheDirectory_CreateOutput_Creates_Output_Stream()
        {
            const string FILE = "myfile.txt";
            using (var output = directory.CreateOutput(FILE))
            { }
            Assert.IsTrue(cache.Get(
                string.Format(
                "{0}\\{1}", DIRECTORY, FILE)) != null);
        }

        [TestMethod]
        public void Test_CacheDirectory_DeleteFile_DeletesFile()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);

            directory.DeleteFile(FILE);
            Assert.IsTrue(cache.Get(
                string.Format(
                "{0}\\{1}", DIRECTORY, FILE)) == null);            
        }

        [TestMethod]
        public void Test_CacheDirectory_FileExists_Detects_File()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);

            Assert.IsTrue(directory.FileExists(FILE));
        }

        [TestMethod]
        public void Test_CacheDirectory_FileModified_Returns_Modified_Time()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);

            var oldModified = directory.FileModified(FILE);

            Assert.AreNotEqual(0, oldModified);
        }

        [TestMethod]
        public void Test_CacheDirectory_ListAll_Returns_All_Items()
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
        public void Test_CacheDirectory_OpenItem_Throws_FileNotFoundException()
        {
            const string FILE = "myfile.txt";
            using (var file = directory.OpenInput(FILE))
            { }
            Assert.Fail();
        }

        [TestMethod]
        public void Test_CacheDirectory_OpenInput_Creates_Input_Stream()
        {
            const string FILE = "myfile.txt";
            CreateFile(FILE);
            using (var file = directory.OpenInput(FILE))
            {
                Assert.IsNotNull(file);
            }
        }

        [TestMethod]
        public void Test_CacheDirectory_TouchFile_Sets_Modified_Time()
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

        public void IndexText(Directory directory, string text)
        {
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);
            IndexWriter indexWriter = new IndexWriter(directory, analyzer, true, new IndexWriter.MaxFieldLength(25000));
            Document document = new Document();
            document.Add(new Field("fieldname", text, Field.Store.YES, Field.Index.ANALYZED));
            indexWriter.AddDocument(document);
            indexWriter.Dispose();
        }

        public Directory CreateFileSystemDirectory(string directory)
        {
            string fileSystemDirectory = directory;
            if (System.IO.Directory.Exists(fileSystemDirectory))
                System.IO.Directory.Delete(fileSystemDirectory, true);
            var fsDirectory = FSDirectory.Open(fileSystemDirectory);
            return fsDirectory;
        }

        public Directory CreateRamDirectory()
        {
            return new RAMDirectory();
        }

        [TestMethod]
        public void Test_CacheDirectory_Control_Writer_Write_Matches_Cache_Writer()
        {
            Directory controlDirectory = CreateRamDirectory();
            var controlOutputFile = controlDirectory.CreateOutput("myfile");
            var cacheOutputFile = directory.CreateOutput("myfile");
            var fakeText = new ET.FakeText.TextGenerator().GenerateText(1000);
            var fakeTextBytes = Encoding.ASCII.GetBytes(fakeText);

            for (int i = 0; i < fakeTextBytes.Length;i++)
            {
                cacheOutputFile.WriteByte(fakeTextBytes[i]);
                controlOutputFile.WriteByte(fakeTextBytes[i]);
            }
            cacheOutputFile.Dispose();
            controlOutputFile.Dispose();

            // begin read phase
            var fsInputFile = controlDirectory.OpenInput("myfile");
            var cacheInputFile = directory.OpenInput("myfile");

            byte[] cacheBuffer = new byte[fakeTextBytes.Length];
            byte[] fsBuffer = new byte[fakeTextBytes.Length];

            cacheInputFile.ReadBytes(cacheBuffer, 0, cacheBuffer.Length);
            fsInputFile.ReadBytes(fsBuffer, 0, fsBuffer.Length);

            for (int b = 0; b < cacheBuffer.Length; b++)
            {
                Assert.AreEqual(cacheBuffer[b], fsBuffer[b],
                    string.Format("Byte {0} expected {1} actual {2}", b, fsBuffer[b], cacheBuffer[b]));
            }            
        }

        [TestMethod]
        public void Test_CacheDirectory_FileSystem_Files_Match_Cache_Files()
        {
            const string text = "This is the text to be indexed.";
            // create the cache index
            IndexText(directory, text);

            Directory controlDirectory = CreateRamDirectory();

            // create the filesystem index            
            IndexText(controlDirectory, text);

            var cacheFiles = directory.ListAll().OrderBy(x=>x).ToArray();
            var fileSystemFiles = controlDirectory.ListAll().OrderBy(x=>x).ToArray();

            // make sure the file counts match
            Assert.AreEqual(cacheFiles.Length, fileSystemFiles.Length);

            for (int i = 0; i < cacheFiles.Length; i++)
            {
                Assert.AreEqual(cacheFiles[i], fileSystemFiles[i]);
                string name = cacheFiles[i];
                var cacheFileStream = directory.OpenInput(name);
                var controlFileStream = controlDirectory.OpenInput(name);
                Assert.AreEqual(cacheFileStream.Length(), controlFileStream.Length());
                int length = Convert.ToInt32(controlFileStream.Length());
                byte[] cacheBuffer = new byte[length];
                byte[] fsBuffer = new byte[length];

                cacheFileStream.ReadBytes(cacheBuffer, 0, cacheBuffer.Length);
                controlFileStream.ReadBytes(fsBuffer, 0, fsBuffer.Length);

                for (int b = 0; b < cacheBuffer.Length; b++)
                {
                    // in the segments file, skip the timestamp
                    if (name == "segments_2")
                        if (4 <= b || b <= 11)
                            break;
                    Assert.AreEqual(cacheBuffer[b], fsBuffer[b], 
                        string.Format("File {0} byte {1} expected {2} actual {3}", name, b, fsBuffer[b], cacheBuffer[b]));
                }
            }
        }

        [TestMethod]
        public void Test_CacheDirectory_Create_And_Read_Index()
        {
            const string TEXT = "This is the text to be indexed.";
            
            // create the index
            Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30);

            // remove
            var controlDirectory = CreateRamDirectory();
            // end remove
            var testDirectory = directory;            

            IndexText(testDirectory, TEXT);
            
            // search the index
            IndexSearcher indexSearcher = new IndexSearcher(testDirectory, true);
            QueryParser parser = new QueryParser(Version.LUCENE_30, "fieldname", analyzer);
            Query query = parser.Parse("text");
            ScoreDoc[] hits = indexSearcher.Search(query, null, 1000).ScoreDocs;
            Assert.AreEqual(1, hits.Length);
                        
            // iterate through the results
            string expectedText = string.Copy(TEXT);
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
