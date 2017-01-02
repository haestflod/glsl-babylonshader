using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using glsl_babylon.classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests.classes
{
    [TestClass]
    public class ShaderStoreTest
    {
        [TestMethod]
        public void GetOutputTest()
        {
            ShaderStore shaderStore = new ShaderStore();
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
            string result = shaderStore.GetOutput(input, "TestShader");
            Assert.IsTrue(result == expectedPretty, "result should equal expected pretty");

            // Minification tests
            shaderStore.DoMinify = true;
            result = shaderStore.GetOutput(input, "TestShader");
            Assert.IsTrue(result == expectedMinified, "result should equal expected minified");
        }
    }
}
