using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using glsl_babylon.classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using tests.testhelpers;
using tests.testhelpers.Interfaces;
using System.IO;
using System.Threading;


namespace tests.classes
{
    [TestClass]
    public class WatcherTest
    {
        public const string WatchDir = "watchtests";
        public const string WatchDirPath = WatchDir + "/";

        private MockConverter m_converter;

        private Watcher m_watcher;        

        [TestInitialize]
        public void Initialize()
        {
            m_converter = new MockConverter();
            m_watcher = new Watcher(m_converter);

            if (!Directory.Exists(WatchDir))
            {
                Directory.CreateDirectory(WatchDir);
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Delete the watch tests directory after each test
            if (Directory.Exists(WatchDir))
            {
                Directory.Delete(WatchDir, true);
            }
        }

        [TestMethod]
        public async Task AddWatcherChangedTest()
        {
            string testfile = "testfile.fx";
            CreateTestFile("line1", testfile);

            m_watcher.AddWatcher(WatchDirPath);

            TestFileHelper.WriteFile(WatchDirPath + testfile, Environment.NewLine + "line2", true);
            //await Task.Run(async () =>
            //{
            //    Thread.Sleep(10);


            await Task.Run(() =>
            {
                Thread.Sleep(10);
                Assert.IsTrue(m_converter.ConvertFileCalls == 1, "Convert was called on file change");
            });
            //});                       
        }

        private void CreateTestFile( string a_content, string a_name)
        {
            string path = WatchDirPath + a_name;            

            File.WriteAllText(path, a_content, System.Text.Encoding.UTF8);
        }
    }
}
