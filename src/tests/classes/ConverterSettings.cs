using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using glsl_babylon.classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace tests.classes
{
    [TestClass]    
    public class ConverterSettingsTest
    {
        
        [TestMethod]
        public void ConstructorTest()
        {
            // Tests for Recursive mode
            ConverterSettings settings = new ConverterSettings("c --r");                  
            Assert.IsTrue(settings.DoRecursiveFolders, "Should do recursive folders");
            Assert.IsTrue(!settings.DoMinify, "Should not minify output");
            Assert.IsTrue(settings.RecursiveDepth == 2, "Recursive depth should be default");

            settings = new ConverterSettings("c --rr -r r");
            Assert.IsTrue(!settings.DoRecursiveFolders, "Should not do recursive folders");
            Assert.IsTrue(!settings.DoMinify, "Should not minify output 2");

            settings = new ConverterSettings("c --minify");
            Assert.IsTrue(settings.DoMinify, "Should minify output");

            settings = new ConverterSettings("c --r --minify");
            Assert.IsTrue(settings.DoRecursiveFolders && settings.DoMinify, "Should do recursive folders and minify");

            settings = new ConverterSettings("c --r 5 --minify");
            Assert.IsTrue(settings.DoRecursiveFolders, "Should do recursive folders 2");
            Assert.IsTrue(settings.RecursiveDepth == 5, "Recursive depth should be changed");
            Assert.IsTrue(settings.Files.Count == 1 && settings.Files[0].Length > 1, "Files should be default path");


            // Tests for output
            settings = new ConverterSettings("c test test2");
            Assert.IsTrue(settings.Files.Count == 2, "Files count should be as expected");
            Assert.IsTrue(settings.Files[0] == "test" && settings.Files[1] == "test2", "Files are expected values");

            settings = new ConverterSettings("c");
            Assert.IsTrue(settings.Files[0] == Application.WorkingDirectory, "Result should be as expected 2");

            settings = new ConverterSettings("c --r");
            Assert.IsTrue(settings.Files[0] == Application.WorkingDirectory, "Result should be as expected 3");

            settings = new ConverterSettings("c test --r --minify test2  test3");
            Assert.IsTrue(settings.Files.Count == 3, "Files count should be as expected");
            Assert.IsTrue(settings.Files[0] == "test" 
                            && settings.Files[1] == "test2" 
                            && settings.Files[2] == "test3", "Files are expected values 3");
            
        }
    }
}
