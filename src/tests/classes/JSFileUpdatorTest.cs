using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using glsl_babylon.classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests.classes
{
    [TestClass]
    public class JSFileUpdatorTest
    {
        private const string TestFolder = "testdata/";

        [TestMethod]
        public void SearchFolderTest()
        {
            JSFileUpdator jsFileUpdator = new JSFileUpdator();

            jsFileUpdator.FindJavascriptFiles(new List<string>() { TestFolder }, 1);

            Assert.IsTrue(jsFileUpdator.m_shaderStores.ContainsKey("TestVertexShader"), "Vertex should be found");
            Assert.IsTrue(jsFileUpdator.m_shaderStores.ContainsKey("TestFragmentShader"), "Fragment should be shader found");

            Assert.IsTrue(false, "Not implemented");
        }
    }
}
