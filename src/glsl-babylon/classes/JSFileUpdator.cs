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
        /// <summary>
        /// Find the key of the shaderstore
        /// </summary>
        // Modified source from: http://stackoverflow.com/questions/23314575/regex-extract-list-of-strings-between-two-strings
        // Changed prefix and added ' or " instead of just "
        private static Regex ShaderStoreKeyRegex = new Regex(@"(?<=BABYLON\.ShaderStore\[(\""|'))[^\""]+(?=(\""|')\])|(?<=\[)[^']+(?='\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public Dictionary<string, ShaderJSFiles> m_shaderStores = new Dictionary<string, ShaderJSFiles>();

        /// <summary>
        /// Find all javascript files in the input folders
        /// </summary>
        /// <param name="a_folders">The list of files/folders</param>
        /// <param name="a_maxDepth">The maxdepth to search for folders</param>
        public void FindJavascriptFiles(List<string> a_folders, int a_maxDepth)
        {
            m_shaderStores.Clear();

            foreach(string path in a_folders)
            {
                if (Directory.Exists(path))
                {
                    SearchFolder(path, 0, a_maxDepth);
                }
            }              
        }

        private void SearchFolder(string a_folder, int a_depth, int a_maxDepth)
        {
            if (a_depth >= a_maxDepth)
            {
                return;
            }

            string[] files = Directory.GetFiles(a_folder, "*.js");

            foreach(string file in files)
            {
                SearchFile(file);
            }

            string[] subdirectories = Directory.GetDirectories(a_folder);
        }

        /// <summary>
        /// Searches files 
        /// </summary>
        /// <param name="a_file"></param>
        private void SearchFile(string a_file)
        {
            string content = File.ReadAllText(a_file);
            if (content.IndexOf("babylon.shaderstore", StringComparison.OrdinalIgnoreCase ) != -1 )
            {
                MatchCollection matches = ShaderStoreKeyRegex.Matches(content);                
                foreach(Match match in matches)
                {
                    if (!m_shaderStores.ContainsKey(match.Value))
                    {
                        m_shaderStores.Add(match.Value, new ShaderJSFiles(a_file));
                    }
                    else
                    {
                        m_shaderStores[match.Value].AddFile(a_file);
                    }
                }                
            }
        }
    }
}
