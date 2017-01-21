using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using glsl_babylon.classes.models;

namespace glsl_babylon.classes
{
    public class JSFileFinder
    {
        /// <summary>
        /// Find the key of the shaderstore
        /// </summary>
        // Modified source from: http://stackoverflow.com/questions/23314575/regex-extract-list-of-strings-between-two-strings
        // Changed prefix and added ' or " instead of just "
        public static Regex ShaderStoreKeyRegex = new Regex(@"(?<=BABYLON\.Effect\.ShadersStore\[(\""|'))[^\""]+(?=(\""|')\])|(?<=\[)[^']+(?='\])", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private HashSet<string> m_searchedFolders = new HashSet<string>();
        
        /// <summary>
        /// Find all javascript files in the input folders
        /// </summary>
        /// <param name="a_folders">The list of files/folders</param>
        /// <param name="a_maxDepth">The maxdepth to search for folders</param>
        public Dictionary<string, ShaderJSFiles> FindJavascriptFiles(List<string> a_folders, int a_maxDepth)
        {
            Dictionary<string, ShaderJSFiles> shaderStores = new Dictionary<string, ShaderJSFiles>();
            m_searchedFolders.Clear();

            foreach (string path in a_folders)
            {
                string directoryName = Path.GetDirectoryName(path);
                if (Directory.Exists(directoryName))
                {
                    SearchFolder(directoryName, 0, a_maxDepth, shaderStores);
                }                
            }

            return shaderStores;
        }

        private void SearchFolder(string a_folder, int a_depth, int a_maxDepth, Dictionary<string, ShaderJSFiles> a_shaderStores)
        {
            if (a_depth >= a_maxDepth)
            {
                return;
            } 
            // This can happen if a user supplies multiple paths that end up searching the same folders
            else if (m_searchedFolders.Contains(a_folder))
            {
                return;
            }
            m_searchedFolders.Add(a_folder);
            // TODO: Typescript files aswell e.t.c.?
            string[] files = Directory.GetFiles(a_folder, "*.js");

            foreach (string file in files)
            {
                SearchFile(file, a_shaderStores);
            }

            string[] subdirectories = Directory.GetDirectories(a_folder);

            foreach(string subdirectory in subdirectories)
            {
                SearchFolder(subdirectory, a_depth + 1, a_maxDepth, a_shaderStores);
            }
        }

        /// <summary>
        /// Searches files 
        /// </summary>
        /// <param name="a_file"></param>
        private void SearchFile(string a_file, Dictionary<string, ShaderJSFiles> a_shaderStore)
        {
            string content = File.ReadAllText(a_file);
            if (content.IndexOf("babylon.effect.shadersstore", StringComparison.OrdinalIgnoreCase) != -1)
            {
                MatchCollection matches = ShaderStoreKeyRegex.Matches(content);
                foreach (Match match in matches)
                {
                    if (!a_shaderStore.ContainsKey(match.Value))
                    {
                        a_shaderStore.Add(match.Value, new ShaderJSFiles(a_file));
                    }
                    else
                    {
                        a_shaderStore[match.Value].AddFile(a_file);
                    }
                }
            }
        }

    }
}
