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
        public void CheckArgumentsTest()
        {

            // Tests for Recursive mode
            m_converter.CheckAndCleanArguments("c --r");
            Assert.IsTrue(m_converter.DoRecursiveFolders, "Should do recursive folders");
            Assert.IsTrue(!m_converter.DoMinify, "Should not minify output");

            m_converter.CheckAndCleanArguments("c --rr -r r");
            Assert.IsTrue(!m_converter.DoRecursiveFolders, "Should not do recursive folders");
            Assert.IsTrue(!m_converter.DoMinify, "Should not minify output 2");

            m_converter.CheckAndCleanArguments("c --minify");
            Assert.IsTrue(m_converter.DoMinify, "Should minify output");

            m_converter.CheckAndCleanArguments("c --r --minify");
            Assert.IsTrue(m_converter.DoRecursiveFolders && m_converter.DoMinify, "Should do recursive folders and minify");
            

            // Tests for output
            string result = m_converter.CheckAndCleanArguments("c test test2");
            Assert.IsTrue(result == "test test2", "Result should be as expected");

            result = m_converter.CheckAndCleanArguments("c");
            Assert.IsTrue(result == AppContext.BaseDirectory, "Result should be as expected 2");

            result = m_converter.CheckAndCleanArguments("c --r");
            Assert.IsTrue(result == AppContext.BaseDirectory, "Result should be as expected 3");

            result = m_converter.CheckAndCleanArguments("c test --r --minify test2  test3");
            Assert.IsTrue(result == "test test2 test3", "Result should be as expected 4");
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

        [TestMethod]
        public void GetOutputTest()
        {
            List<string> input = new List<string>() {
                "uniform vec3 uTest",
                "void main()",
                "{",
                "gl_Position = vPosition",
                "}"
            };

            string newline = Environment.NewLine;

            string expectedPretty = string.Format("BABYLON.Effect.ShadersStore[\"TestShader\"]={0}"
                + "\"uniform vec3 uTest\"+{0}"
                + "\"void main()\"+{0}"
                + "\"{{\"+{0}"
                + "\"gl_Position = vPosition\"+{0}"
                + "\"}}\";", newline);

            string expectedMinified = string.Format("BABYLON.Effect.ShadersStore[\"TestShader\"]="
                + "\"uniform vec3 uTest\"+"
                + "\"void main()\"+"
                + "\"{{\"+"
                + "\"gl_Position = vPosition\"+"
                + "\"}}\";");

            // Pretty print tests
            m_converter.DoMinify = false;
            string result = m_converter.GetOutput(input, "TestShader" );
            Assert.IsTrue(result == expectedPretty, "result should equal expected pretty");

            // Minification tests
            m_converter.DoMinify = true;
            result = m_converter.GetOutput(input, "TestShader");
            Assert.IsTrue(result == expectedMinified, "result should equal expected minified");
        }

    }
}
