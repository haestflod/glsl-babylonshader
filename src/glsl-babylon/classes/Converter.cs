using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

namespace glsl_babylon.classes
{
    public class Converter
    {
        // Courtesy of James0x57
        Regex m_lineComment = new Regex(@"\/\/.*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        // This is to separate instances of *//* that the lineComment would remove completly
        // No one probably writes /* comment *//* second comment */ but hey!
        Regex m_twoBlockComments = new Regex(@"\*\/\/\*");
        // Replace multiple spaces with 1
        // For example the twoBlockComments regex adds spaces 
        // Source: http://stackoverflow.com/questions/206717/how-do-i-replace-multiple-spaces-with-a-single-space-in-c
        Regex m_multipleSpaces = new Regex("[ ]{2,}");        
        // If is inside a block comment currently or not
        bool m_insideBlockComment = false;

        // How many files in total has been tried to convert
        public int Converted { get; set; } = 0;
        // How many succeeded
        public int Success { get; set; } = 0;
        // How many failed
        public int Failed { get; set; } = 0;

        /// <summary>
        /// Takes the action string and splits it and then tries to convert both folders and files.
        /// </summary>
        /// <param name="a_action"></param>
        public void Convert(string a_action)
        {
            string[] parts = a_action.Split(' ');

            Converted = Success = Failed = 0;       

            for (int i = 1; i < parts.Length; ++i)
            {
                // If folder "test" exists it will convert that.
                // If files with test.vertex.fx | test.fragment.fx exists it will convert those  
                // Then it will convert the whole testfolder and the test.vertex.fx & test.fragment.fx
                string part = parts[i];
                // Convert a potential folder
                bool convertedFolder = TryConvertFolder(part);
                // Check if the part ends with .fx and it wasn't a folder called *.fx      
                // Also check if it ends with .fragment or .vertex          
                bool convertedFiles = TryConvertFxFile(part);  
                 
            }

            if (parts.Length > 1)
            {
                Application.PrintLine(String.Format("Converted {0} files. Succesfully converted {1}", Converted, Success), ConsoleColor.Green);
            }
            else
            {
                Application.PrintLine("There were no files as input.", ConsoleColor.Yellow);
            }
        }

        /// <summary>
        /// Converts all files in a directory and recursively too.
        /// </summary>
        /// <param name="a_foldername"></param>
        /// <returns></returns>
        public bool TryConvertFolder(string a_foldername)
        {
            if (Directory.Exists( a_foldername))
            {

            }

            return false;
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
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();

                            string result = ParseLine(line).Trim();

                            if (!string.IsNullOrWhiteSpace(result))
                            {
                                lines.Add(result);
                            }
                        }                        
                    }
                }

                // If there are no lines it failed to convert
                if (lines.Count > 0)
                {
                    using (FileStream fs = new FileStream(a_filename + ".output", FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.Write(GetOutput(lines));
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
        /// Get the babylon shaderstore output
        /// </summary>
        /// <param name="a_values"></param>
        /// <returns></returns>
        public string GetOutput(List<string> a_values)
        {            
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < a_values.Count; i++)
            {
                string value = m_multipleSpaces.Replace(a_values[i], " ");
                // Reason for adding Environment.NewLine is so that when you copy-paste into your editor it'll be on a new line!
                if (i < a_values.Count - 1)
                {
                    // "glsl"+
                    sb.AppendFormat("\"{0}\"+{1}", value, Environment.NewLine);
                }
                else
                {
                    // "glsl";
                    sb.AppendFormat("\"{0}\";", value);
                }                
            }

            return sb.ToString();
        }

        

        private string ParseLine( string a_line)
        {            
            string result = "";
            // If not a block comment is active
            if (!m_insideBlockComment)
            {
                a_line = m_twoBlockComments.Replace(a_line, "*/ /*");
                a_line = m_lineComment.Replace(a_line, "");
                a_line = a_line.Trim();

                // Check if line contains block comment after line comment removal
                if (a_line.Contains("/*"))
                {
                    m_insideBlockComment = true;

                    int startIndex = a_line.IndexOf("/*");
                    // Check if the line contains */ aswell
                    // float x = 1.0; /* comment */
                    if (a_line.Contains( "*/"))
                    {        
                        // Add +2 to get the point after /*                
                        int endIndex = a_line.IndexOf("*/")+2 ;
                        string part1 = a_line.Substring(0, startIndex);
                        string part2 = a_line.Substring(endIndex);

                        m_insideBlockComment = false;
                        // Do another parse 
                        // For example 
                        // float x = 1.0; /* comment */ float y = 1.0; /* 2ndComment */
                        result = ParseLine(part1 + part2);
                    }
                    else
                    {
                        result = a_line.Substring(0, startIndex);
                    }
                }
                else
                {
                    result = a_line;
                }
            }            
            else
            {
                // if block comment is active check if the current line contains */
                if (a_line.Contains("*/"))
                {
                    // If so substring it away and then parse line again
                    int endIndex = a_line.IndexOf("*/") + 2;
                    m_insideBlockComment = false;
                    result = ParseLine(a_line.Substring(endIndex));
                }
            }

            return result;
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
