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
        public static void CopyFiles(string a_folder, string a_outputFolder, string a_prefix = "")
        {
            string[] files = Directory.GetFiles(a_folder);
            string[] directories = Directory.GetDirectories(a_folder);

            string outputFolder = a_outputFolder + a_prefix;

            if ( !Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            outputFolder += "/";

            foreach (string file in files)
            {
                File.Copy(file, outputFolder + Path.GetFileName(file), true);
            }

            foreach(string directory in directories)
            {
                string directoryName = directory.Split(new char[] { '/', '\\' }).Last();
                CopyFiles(directory, a_outputFolder, a_prefix + "/" + directoryName);
            }
        }

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

        public static StreamReader GetFileContentStream( string a_file )
        {
            if (File.Exists(a_file))
            {
                FileStream fs = new FileStream(a_file, FileMode.Open);                
                StreamReader sr = new StreamReader(fs, Encoding.UTF8);                    
                return sr;
                    
            }

            return null;
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
