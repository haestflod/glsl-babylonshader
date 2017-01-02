using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace glsl_babylon.classes
{
    public class ConverterSettings
    {
        public const string RecursiveAction = "--r";

        public const string MinifyAction = "--minify";

        public bool DoRecursiveFolders { get; private set; } = false;
        public int RecursiveDepth { get; private set; } = 2;
        public bool DoMinify { get; private set; } = false;

        public List<string> Files { get; private set; } = new List<string>();

        public ConverterSettings(string a_arguments)
        {
            /*
             * Any arguments that comes in will either be:
             * 1) An option like --minify to minify the output
             * 2) A filepath
             * All files will be added to Files list
             */
           
            string[] parts = a_arguments.Split(' ');                   

            for (int i = 1; i < parts.Length; i++)
            {
                string part = parts[i];

                if (part == RecursiveAction)
                {
                    DoRecursiveFolders = true;
                    // Check for potential depth
                    if (i + 1 < parts.Length)
                    {
                        string depthpart = parts[i + 1];
                        int depth;
                        // Could in theory do i++ but if a file was a digit it'd get removed from output
                        if (int.TryParse(depthpart, out depth))
                        {
                            // -1, -2, -3  all mean the same thing
                            if (depth < 0)
                            {
                                depth = -1;
                            }
                            RecursiveDepth = depth;
                            // Increase i by 1 since we've dealt with that number.
                            i++;
                        }
                    }

                }
                else if (part == MinifyAction)
                {
                    DoMinify = true;
                }
                else if (part != "")
                {
                    Files.Add(part);
                }
            }

            // Get current working directory if files count is 0
            if (Files.Count == 0)
            {                
                Files.Add(AppContext.BaseDirectory);
            }
        }
    }
}
