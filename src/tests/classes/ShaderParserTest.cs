using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using glsl_babylon.classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tests.testhelpers;

namespace tests.classes
{
    [TestClass]
    public class ShaderParserTest
    {
        /// <summary>
        /// The file to convert
        /// </summary>
        private const string TestFileInput = "testdata/test.vertex.fx";

        /// <summary>
        /// The file to expected content of the converter
        /// </summary>
        private const string TestFileExpected = "testdata/test.vertex.parsed.fx";


        [TestMethod]        
        public void ParseFileTest()
        {
            ShaderParser parser = new ShaderParser();

            List<string> resultList = new List<string>();
            StreamReader inputStream = TestFileHelper.GetFileContentStream(TestFileInput);
            parser.ParseFile(inputStream, resultList);

            string resultString = String.Join(Environment.NewLine, resultList);           
            string expectedContent = TestFileHelper.GetFileContent(TestFileExpected);            

            // Check the output
            Assert.IsTrue(resultList.Count > 0, "Should have parsed some lines");
            Assert.IsTrue(resultString == expectedContent, "Parsed content should equal expected");           
            
        }
    }
}
