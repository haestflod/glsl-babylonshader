using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using glsl_babylon.classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests.classes
{
    [TestClass]
    public class JSFileFinderTest
    {
        private const string TestFolder = "testdata/";

        [TestMethod]
        public void SearchFolderTest()
        {
            JSFileFinder jsFileUpdator = new JSFileFinder();

            var result = jsFileUpdator.FindJavascriptFiles(new List<string>() { TestFolder }, 1);

            Assert.IsTrue(result.ContainsKey("TestVertexShader"), "Vertex should be found");
            Assert.IsTrue(result.ContainsKey("TestFragmentShader"), "Fragment should be found");
            Assert.IsTrue(!result.ContainsKey("Test2VertexShader"), "Test2 Vertex for depth 2 should not be found");

            Assert.IsTrue(result["TestVertexShader"].Files.Count == 1, "Should be 1 TestVertexShader files");
            Assert.IsTrue(result["TestFragmentShader"].Files.Count == 1, "Should be 1 TestVertexShader files");

            Assert.IsTrue(result["TestVertexShader"].Files[0].EndsWith("testdata\\test.js"), "TestVertexShader is correct file");
            Assert.IsTrue(result["TestFragmentShader"].Files[0].EndsWith("testdata\\test.js"), "TestFragmentShader is correct file");

            result = jsFileUpdator.FindJavascriptFiles(new List<string>() { TestFolder }, 2);

            Assert.IsTrue(result.ContainsKey("TestVertexShader"), "Vertex should be found 2");
            Assert.IsTrue(result.ContainsKey("TestFragmentShader"), "Fragment should be found 2");
            Assert.IsTrue(result.ContainsKey("Test2VertexShader"), "Test2 Vertex for depth 2 should be found 2");

            Assert.IsTrue(result["TestVertexShader"].Files.Count == 1, "Should be 1 TestVertexShader files 2");
            Assert.IsTrue(result["TestFragmentShader"].Files.Count == 2, "Should be 2 TestVertexShader files 2");
            Assert.IsTrue(result["Test2VertexShader"].Files.Count == 1, "Should be 1 TestVertexShader files 2");

            Assert.IsTrue(result["TestVertexShader"].Files[0].EndsWith("testdata\\test.js"), "TestVertexShader is correct file 2");            
            Assert.IsTrue(result["TestFragmentShader"].Files[0].EndsWith("testdata\\test.js"), "TestFragmentShader is correct file 2");
            Assert.IsTrue(result["TestFragmentShader"].Files[1].EndsWith("testdata\\testdata2\\test2.js"), "TestFragmentShader 2nd file is correct file 2");
            Assert.IsTrue(result["Test2VertexShader"].Files[0].EndsWith("testdata\\testdata2\\test2.js"), "Test2VertexShader file is correct file 2");

            result = jsFileUpdator.FindJavascriptFiles(new List<string>() { TestFolder + "test.js" }, 1);
            Assert.IsTrue(result.ContainsKey("TestVertexShader"), "Vertex should be found 3");
            Assert.IsTrue(result.ContainsKey("TestFragmentShader"), "Fragment should be found 3");

            result = jsFileUpdator.FindJavascriptFiles(new List<string>() { TestFolder + "test.fragment.fx" }, 1);
            Assert.IsTrue(result.ContainsKey("TestVertexShader"), "Vertex should be found 4");
            Assert.IsTrue(result.ContainsKey("TestFragmentShader"), "Fragment should be found 4");

            result = jsFileUpdator.FindJavascriptFiles(new List<string>() { TestFolder + "moomin/" }, 1);
            Assert.IsTrue(!result.ContainsKey("TestVertexShader"), "Vertex should not be found 5");
            Assert.IsTrue(!result.ContainsKey("TestFragmentShader"), "Fragment should not be found 5");


        }
    }
}
