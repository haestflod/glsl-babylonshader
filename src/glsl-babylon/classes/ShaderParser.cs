using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace glsl_babylon.classes
{
    public class ShaderParser
    {
        // If is inside a block comment currently or not
        private bool m_insideBlockComment = false;

        // Courtesy of James0x57
        private static Regex LineComment = new Regex(@"\/\/.*$", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        // This is to separate instances of *//* that the lineComment would remove completly
        // No one probably writes /* comment *//* second comment */ but hey!
        private static Regex TwoBlockComments = new Regex(@"\*\/\/\*", RegexOptions.Compiled);

        // Replace multiple spaces with 1
        // For example the twoBlockComments regex adds spaces 
        // Source: http://stackoverflow.com/questions/206717/how-do-i-replace-multiple-spaces-with-a-single-space-in-c
        private static Regex MultipleSpaces = new Regex("[ ]{2,}", RegexOptions.Compiled);

        public void ParseFile(StreamReader a_stream, List<string> a_lines)
        {
            if (a_stream != null)
            {
                while (!a_stream.EndOfStream)
                {
                    string line = a_stream.ReadLine();

                    string result = ParseLine(line).Trim();

                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        result = MultipleSpaces.Replace(result, " ");
                        a_lines.Add(result);
                    }
                }
            }            
        }

        /// <summary>
        /// Parse a line of shader code
        /// </summary>
        /// <param name="a_line"></param>
        /// <returns></returns>
        private string ParseLine(string a_line)
        {
            string result = "";
            // If not a block comment is active
            if (!m_insideBlockComment)
            {
                a_line = TwoBlockComments.Replace(a_line, "*/ /*");
                a_line = LineComment.Replace(a_line, "");
                a_line = a_line.Trim();

                // Check if line contains block comment after line comment removal
                if (a_line.Contains("/*"))
                {
                    m_insideBlockComment = true;

                    int startIndex = a_line.IndexOf("/*");
                    // Check if the line contains */ aswell
                    // float x = 1.0; /* comment */
                    if (a_line.Contains("*/"))
                    {
                        // Add +2 to get the point after /*                
                        int endIndex = a_line.IndexOf("*/") + 2;
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
