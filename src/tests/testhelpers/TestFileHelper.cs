using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace tests.testhelpers
{
    public static class TestFileHelper
    {
        public static string GetFileContent( string a_file)
        {
            string content = "";
            if (File.Exists( a_file))
            {
                using (FileStream fs = new FileStream( a_file, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader( fs))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        } 
    }
}
