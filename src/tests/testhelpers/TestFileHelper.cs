using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                    using (StreamReader sr = new StreamReader( fs, Encoding.UTF8))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }
        
        public static void WriteFile( string a_file, string a_content, bool a_append = false)
        {
            FileMode mode = a_append ? FileMode.Append : FileMode.Create;

            using (FileStream fs = new FileStream(a_file, mode))
            {
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8 ))
                {
                    sw.WriteLine(a_content);
                }
            }
        } 
    }
}
