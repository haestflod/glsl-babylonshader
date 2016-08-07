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
        // If is inside a block comment currently or not
        bool m_insideBlockComment = false;

        public int Converted { get; set; } = 0;
        public int Success { get; set; } = 0;
        public int Failed { get; set; } = 0;

        public void Convert(string a_action)
        {
            string[] parts = a_action.Split(' ');

            Converted = Success = Failed = 0;       

            for (int i = 1; i < parts.Length; ++i)
            {
                // TODO: Check if folder

                // TODO: Check if files exist with .vertex.fx  or .fragment.fx
                Converted++;
                if (ConvertFile(parts[i]))
                {
                    Success++;
                }
                else
                {
                    Failed++;   
                }
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

        public bool ConvertFile(string a_filename)
        {
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
                    return true;
                }
                
            }
            // If file doesn't exist
            else
            {
                Application.PrintLine("File did not exist: " + a_filename, ConsoleColor.Red);                
            }  

            return false;
        }

        public string GetOutput(List<string> a_values)
        {            
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < a_values.Count; i++)
            {                
                // Reason for adding Environment.NewLine is so that when you copy-paste into your editor it'll be on a new line!
                if (i < a_values.Count - 1)
                {
                    // "text"+
                    sb.AppendFormat("\"{0}\"+{1}", a_values[i], Environment.NewLine);
                }
                else
                {
                    // "text";
                    sb.AppendFormat("\"{0}\";", a_values[i]);
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
    }
}
