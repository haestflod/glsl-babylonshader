using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace glsl_babylon.classes.models
{
    public class ShaderJSFiles
    {
        public List<string> Files { get; set; } = new List<string>();

        public ShaderJSFiles()
        {

        }

        public ShaderJSFiles(string a_file)
        {
            AddFile(a_file);
        }

        public bool AddFile(string a_file)
        {
            if (!Files.Contains(a_file))
            {
                Files.Add(a_file);
                return true;
            }
            return false;
        }
    }
}
