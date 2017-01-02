using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace glsl_babylon.classes
{
    public class Converter : IConverter
    {                    
        ShaderParser m_shaderParser = new ShaderParser();
        ShaderStore m_shaderStore = new ShaderStore();

        // How many files in total has been tried to convert
        public int Converted { get; set; } = 0;
        // How many succeeded
        public int Success { get; set; } = 0;
        // How many failed
        public int Failed { get; set; } = 0;
        
        /// <summary>
        /// If convertFolder should do it recursively or just the input folder
        /// </summary>
        public bool DoRecursiveFolders { get; set; } = false;
        /// <summary>
        /// How many folders deep to do the recursive action.
        /// For example if a user mistakenly writes "/" well we don't want to loop through whole system!
        /// -1 is until no more folders
        /// </summary>
        public int RecursiveDepth { get; set; } = 2;

        public void ParseSettings(ConverterSettings a_settings)
        {
            DoRecursiveFolders = a_settings.DoRecursiveFolders;
            RecursiveDepth = a_settings.RecursiveDepth;

            m_shaderStore.DoMinify = a_settings.DoMinify;
        }

        /// <summary>
        /// Takes the arguments string and splits it and then tries to convert both folders and files.
        /// </summary>
        /// <param name="a_arguments"></param>
        public void Convert(string a_arguments)
        {
            ConverterSettings settings = new ConverterSettings(a_arguments);
            ParseSettings(settings);
            
            Converted = Success = Failed = 0;

            // Try catch since we're doing IO stuff!
            try
            {
                foreach (string file in settings.Files)
                {
                    // If folder "test" exists it will convert that.
                    // If files with test.vertex.fx | test.fragment.fx exists it will convert those  
                    // Then it will convert the whole testfolder and the test.vertex.fx & test.fragment.fx
                    

                    // Convert a potential folder
                    bool convertedFolder = TryConvertFolder(file, 0);

                    // Check if the part ends with .fx and it wasn't a folder called *.fx      
                    // Also check if it ends with .fragment or .vertex          
                    bool convertedFiles = TryConvertFxFile(file);
                }                

                if (Converted > 0)
                {
                    Application.PrintLine(String.Format("Converted {0} files. Succesfully converted {1}", Converted, Success), ConsoleColor.Green);
                }
                else
                {
                    Application.PrintLine("There were no files as input.", ConsoleColor.Yellow);
                }
            }
            catch( Exception e)
            {
                Application.PrintLine("Unexpected error: " + e.Message, ConsoleColor.Red);
            }            
        }            

        /// <summary>
        /// Converts a file 
        /// </summary>
        /// <param name="a_filename"></param>
        /// <returns></returns>
        public bool ConvertFile(string a_filename)
        {
            Converted++;
            if (File.Exists(a_filename))
            {
                List<string> lines = new List<string>();
                using (FileStream fs = new FileStream(a_filename, FileMode.Open))
                {
                    // Seems .net core doesn't take a filename as input?             
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        m_shaderParser.ParseFile(sr, lines);                
                    }
                }

                // If there are no lines it failed to convert
                if (lines.Count > 0)
                {                    
                    using (FileStream fs = new FileStream(a_filename + ".output", FileMode.Create))
                    {
                        // TODO: Implement automatic encoding detection?
                        // http://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                        {
                            string shaderStoreName = m_shaderStore.GetShaderStoreName( a_filename );
                            string shaderOutput = m_shaderStore.GetOutput(lines, shaderStoreName);
                            sw.Write(shaderOutput);
                        }
                    }
                    // Before returning true increase successful convertion!
                    Success++;
                    return true;
                }
                
            }
            // If file doesn't exist
            else
            {
                Application.PrintLine("File did not exist: " + a_filename, ConsoleColor.Red);                
            }
            // If failing then increase failures
            Failed++;
            return false;
        }       

        /// <summary>
        /// Converts all files in a directory and recursively too.
        /// </summary>
        /// <param name="a_foldername"></param>
        /// <returns></returns>
        public bool TryConvertFolder(string a_foldername, int a_depth)
        {
            // Set default to false and if at least 1 file succeeds it will return true
            bool fileSuccess = false;
            bool folderSuccess = false;
            if (Directory.Exists( a_foldername))
            {
                // Get all .fx files
                string[] files = Directory.GetFiles(a_foldername, "*.fx");

                foreach (string file in files)
                {
                    if ( ConvertFile(file))
                    {
                        fileSuccess = true;
                    }
                }

                if (DoRecursiveFolders && (a_depth == -1 || a_depth < RecursiveDepth))
                {
                    string[] subdirectories = Directory.GetDirectories(a_foldername);

                    foreach (string subdirectory in subdirectories)
                    {
                        // Recursively call this function and increase depth by 1
                        if ( TryConvertFolder(subdirectory, a_depth + 1))
                        {
                            folderSuccess = true;
                        }
                    }
                }
            }            

            return fileSuccess || folderSuccess;
        }

        

        /// <summary>
        /// Try first to convert input filename
        /// If that file doesn't exist then add .fragment.fx and .vertex.fx and try those combinations
        /// </summary>
        /// <param name="a_file"></param>
        /// <returns></returns>
        private bool TryConvertFxFile(string a_file)
        {
            // If absolute path to file.
            // Like shader.glsl           
            if (File.Exists( a_file))
            {
                return ConvertFile(a_file);
            }
            else
            {
                // If the file didn't exist then add .vertex.fx and .fragment.fx
                string vertexShader = a_file + ".vertex.fx";
                string fragmentShader = a_file + ".fragment.fx";

                bool vertexResult = false;
                bool fragmentResult = false;

                if (File.Exists( vertexShader ))
                {
                    vertexResult = ConvertFile(vertexShader);
                }
                if (File.Exists(fragmentShader))
                {
                    fragmentResult = ConvertFile(fragmentShader);
                }
                // Returns true if either vertex of fragment succeeded
                return vertexResult || fragmentResult;
            }           
        }
    }
}
