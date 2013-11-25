using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace CacheIt.Lucene.UnitTests
{
    [TestClass]
    public class CacheInputStreamTests
    {

        [TestMethod]
        public void Test_CacheInputStream_Read()
        {
            string testString = new ET.FakeText.TextGenerator().GenerateText(100);
            byte[] bytes = Encoding.ASCII.GetBytes(testString);
            byte[] buffer = new byte[1024];
        }
    }
}
