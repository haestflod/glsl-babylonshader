using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using glsl_babylon.classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tests.testhelpers;

namespace tests.classes
{
    [TestClass]
    public class JSFileUpdatorTest
    {
        private const string TestFolder = "testdata/";
        private const string TestCopiedFolder = "testdatacopy";

        private static string TestUpdated1Expected = TestCopiedFolder + "/test.updated1.expected";
        private static string TestUpdated2Expected = TestCopiedFolder + "/test.updated2.expected";

        [TestCleanup]
        public void Cleanup()
        {
            // Since they share the same folder cleanup after each test
            if (Directory.Exists(JSFileUpdator.BackupRelativePath))
            {
                Directory.Delete(JSFileUpdator.BackupRelativePath, true);
            }
            if (Directory.Exists(TestCopiedFolder))
            {
                Directory.Delete(TestCopiedFolder, true);
            }
        }        

        [TestMethod]
        public void TryUpdateFiles()
        {
            TestFileHelper.CopyFiles(TestFolder, TestCopiedFolder);

            string expected1 = TestFileHelper.GetFileContent(TestUpdated1Expected);
            string expected2 = TestFileHelper.GetFileContent(TestUpdated2Expected);
            string testfile = TestCopiedFolder + "/test.js";


            JSFileUpdator fileUpdator = new JSFileUpdator();
            fileUpdator.ShaderStores = new Dictionary<string, glsl_babylon.classes.models.ShaderJSFiles>();
            fileUpdator.ShaderStores.Add("TestVertexShader", new glsl_babylon.classes.models.ShaderJSFiles()
            {
                Files = new List<string>() { testfile }
            });
            // Start real simple for first test with text output!
            fileUpdator.TryUpdateFiles("TestVertexShader", "BABYLON.Effect.ShaderStore[\"TestVertexShader\"] = \"test\";");

            string result = TestFileHelper.GetFileContent(testfile);

            Assert.IsTrue(result == expected1, "result equals expected result");
            Assert.IsTrue(File.Exists(JSFileUpdator.BackupRelativePath + "/" + testfile + "-1.backup"), "The testfile was copied");

            // Test 2
            fileUpdator.ShaderStores.Add("TestFragmentShader", new glsl_babylon.classes.models.ShaderJSFiles()
            {
                Files = new List<string>() { testfile }
            });
            fileUpdator.ShaderStores.Add("InvalidVertexShader", new glsl_babylon.classes.models.ShaderJSFiles()
            {
                Files = new List<string>() { testfile }
            });
            // Reset the files
            TestFileHelper.CopyFiles(TestFolder, TestCopiedFolder);

            // Start real simple for first test with text output!
            fileUpdator.TryUpdateFiles("TestVertexShader", "BABYLON.Effect.ShaderStore[\"TestVertexShader\"] =\r\n\"attribute vec3 aPosition;\"+\r\n\"attribute vec2 aUV;\";");
            // Start real simple for first test with text output!
            fileUpdator.TryUpdateFiles("TestFragmentShader", "BABYLON.Effect.ShaderStore[\"TestFragmentShader\"] = \"changed fragmentshader\";");

            result = TestFileHelper.GetFileContent(testfile);

            Assert.IsTrue(result == expected2, "result equals expected result");
            Assert.IsTrue(File.Exists(JSFileUpdator.BackupRelativePath + "/" + testfile + "-2.backup"), "The testfile was copied and increased");
        }

        [TestMethod]
        public void BackupFileTest()
        {
            string testfile = TestFolder+ "test.js";
            JSFileUpdator fileUpdator = new JSFileUpdator();
            fileUpdator.BackupFile(testfile);

            Assert.IsTrue(File.Exists(JSFileUpdator.BackupRelativePath + "/" + testfile + "-1.backup"), "The testfile was copied");

            fileUpdator.BackupFile(testfile);
            fileUpdator.BackupFile(testfile);

            Assert.IsTrue(File.Exists(JSFileUpdator.BackupRelativePath + "/" + testfile + "-2.backup"), "The testfile was copied 2");
            Assert.IsTrue(File.Exists(JSFileUpdator.BackupRelativePath + "/" + testfile + "-3.backup"), "The testfile was copied 3");
        }
    }
}
