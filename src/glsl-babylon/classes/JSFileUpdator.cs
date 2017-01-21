using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using glsl_babylon.classes.models;

namespace glsl_babylon.classes
{
    public class JSFileUpdator
    {
        public const string BackupRelativePath = "glsl-babylonshader-backups";
        public const string ShaderStoreVariable = "babylon.effect.shadersstore";


        public Dictionary<string, ShaderJSFiles> ShaderStores { get; set; }

        public void TryUpdateFiles(string a_shaderStoreName, string a_shaderOutput)
        {
            if (ShaderStores.ContainsKey(a_shaderStoreName))
            {
                ShaderJSFiles jsFiles = ShaderStores[a_shaderStoreName];
                foreach(string file in jsFiles.Files)
                {
                    TryUpdateFile(file, a_shaderStoreName, a_shaderOutput);
                }
            }
        }

        // Public so easier to unit test!
        public void BackupFile(string a_file)
        {
            string filename = Path.GetFileName(a_file);
            string directory = BackupRelativePath + "/" + Path.GetDirectoryName(a_file);
            
                     

            // Start with it being first backup
            int currentId = 1;

            string backupname = String.Format("{0}/{1}-{2}.backup", directory, filename, currentId++);

            while ( File.Exists(backupname))
            {
                backupname = String.Format("{0}/{1}-{2}.backup", directory, filename, currentId++);
            }

            if ( !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(a_file, backupname, true);
        }

        public int GetShaderStoreLength(string a_content, int a_startIndex, string a_shaderStoreName)
        {            

            int shaderStoreNameIndex = a_content.IndexOf(a_shaderStoreName);
            int nextShaderStore = a_content.IndexOf(ShaderStoreVariable, StringComparison.OrdinalIgnoreCase);

            if (shaderStoreNameIndex != -1 && (nextShaderStore == -1 || (shaderStoreNameIndex < nextShaderStore)))
            {                
                char currentQuote = '"';
                bool firstEqual = false;
                bool insideQuotes = false;

                string parsed = "";

                for (int i = shaderStoreNameIndex + a_shaderStoreName.Length; i < a_content.Length; i++)
                {
                    char letter = a_content[i];
                    parsed += letter;

                    if ( !firstEqual)
                    {
                        // Without this the code below registered the first quote mark:
                        // "] = ...
                        if (letter == '=')
                        {
                            firstEqual = true;
                        }
                    }
                    else
                    {
                        // Reached the end!
                        if (!insideQuotes)
                        {
                            if (letter == ';')
                            {
                                // Add ShaderStoreVariable.Length because it was removed from a_content
                                // +1 to remove the extra ;
                                return i + ShaderStoreVariable.Length + 1;
                            }
                            else if (letter == '\'' || letter == '"')
                            {
                                // Check that the previous character isn't \ for backslashing \" \'
                                if (a_content[i - 1] != '\\')
                                {
                                    currentQuote = letter;
                                    insideQuotes = true;
                                }
                            }
                        }
                        else
                        {
                            if (letter == currentQuote)
                            {
                                insideQuotes = false;
                            }
                        }
                    }

                    
                }
            }

            return -1;
        }

        private bool TryUpdateFile(string a_file, string a_shaderStoreName, string a_shaderOutput)
        {
            if ( File.Exists(a_file) )
            {
                // 1. Backup current file
                // 2. Find the startpos and endpos to replace text with
                // 3. write new shaderOutput at that position                

                string content = File.ReadAllText(a_file);                

                List<int> startIndices = new List<int>();
                List<int> lengths = new List<int>();                

                int startIndex = content.IndexOf(ShaderStoreVariable, StringComparison.OrdinalIgnoreCase);
                int contentIndex = 0;

                while (startIndex != -1)
                {
                    contentIndex += startIndex + ShaderStoreVariable.Length;
                    string startContent = content.Substring(contentIndex);
                    int length = GetShaderStoreLength(startContent, startIndex, a_shaderStoreName);

                    if (length != -1)
                    {
                        // Remove ShaderStoreVariable.Length as it was added earlier
                        startIndices.Add(contentIndex - ShaderStoreVariable.Length);
                        // The length has the SSV.Length added
                        lengths.Add(length);
                    }

                    startIndex = startContent.IndexOf(ShaderStoreVariable, StringComparison.OrdinalIgnoreCase);
                }                
                
                if (startIndices.Count > 0)
                {
                    // Only do backup of file if any changes should be made
                    BackupFile(a_file);
                    string newContent = "";
                    int currentIndex = 0;

                    for (int i = 0; i < startIndices.Count; i++)
                    {
                        startIndex = startIndices[i];
                        int length = lengths[i];

                        newContent += content.Substring(currentIndex, startIndex);
                        newContent += a_shaderOutput;

                        currentIndex = startIndex + length;
                    }

                    newContent += content.Substring(currentIndex);

                    File.WriteAllText(a_file, newContent);

                    Application.PrintLine(String.Format("Updated javascript file {0} with new shadersstore code.", a_file), ConsoleColor.White);
                    return true;
                }                              
            }

            return false;
        }

        
    }
}
