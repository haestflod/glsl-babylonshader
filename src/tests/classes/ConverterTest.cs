using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using glsl_babylon.classes;
using tests.testhelpers;
using System.IO;

namespace tests.classes
{
    [TestClass]
    public class ConverterTest
    {
        private Converter m_converter;

        /// <summary>
        /// The file to convert
        /// </summary>
        private const string TestFileInput = "testdata/test.vertex.fx";
        /// <summary>
        /// The file that the converter will output, the result of the converter
        /// </summary>
        private const string TestFileResult = "testdata/test.vertex.fx.output";
        /// <summary>
        /// The file to expected content of the converter
        /// </summary>
        private const string TestFileExpected = "testdata/test.vertex.fx.expected";

        [TestInitialize]
        public void Initialize()
        {
            m_converter = new Converter();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Since they share the same folder cleanup after each test
            if (File.Exists( TestFileResult))
            {
                File.Delete(TestFileResult);
            }
        }        

        [TestMethod]
        public void ConvertTest()
        {
            m_converter.Convert("c");

            Assert.IsTrue(!File.Exists(TestFileResult), "Test result file should not exist");
            
            m_converter.Convert("null");

            Assert.IsTrue(!File.Exists(TestFileResult), "Test result file should not exist 2");

            m_converter.Convert("c " + TestFileInput);

            Assert.IsTrue(File.Exists(TestFileResult), "Test result file should exist");
            File.Delete(TestFileResult);

            m_converter.Convert("c " + TestFileInput + " notfound.fx");
            Assert.IsTrue(File.Exists(TestFileResult), "Test result file should exist 2");
            Assert.IsTrue(!File.Exists("notfound.fx"), "notfound.fx.output should not exist 2");
        }

        [TestMethod]
        public void ConvertFileTest()
        {
            bool result404 = m_converter.ConvertFile("notfound.fx");

            Assert.IsTrue(!result404, "Should not succeed for a file that doesn't exist" );
            
            bool resultOK = m_converter.ConvertFile(TestFileInput);

            // Check the output
            Assert.IsTrue(resultOK, "Converted should succeed");
            string resultContent = TestFileHelper.GetFileContent(TestFileResult);
            string expectedContent = TestFileHelper.GetFileContent(TestFileExpected);
            
            Assert.IsTrue(resultContent == expectedContent, "Converted content should be expected content");          
        }        

    }
}
