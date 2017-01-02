using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace glsl_babylon.classes
{
    public class Watcher
    {
        /// <summary>
        /// The converter used to convert files after IO update event happens
        /// </summary>
        private IConverter m_converter;

        private Dictionary<string, FileSystemWatcher> m_watchedPaths;

        private FileSystemWatcher watcher;

        public Watcher(IConverter a_converter)
        {
            m_converter = a_converter;

            m_watchedPaths = new Dictionary<string, FileSystemWatcher>();
        }

        /// <summary>
        /// Adds 
        /// </summary>
        /// <param name="a_path"></param>
        public void AddWatcher( string a_path)
        {
            // Make sure it's not already watched
            if (!m_watchedPaths.ContainsKey( a_path ))
            {                
                if (Directory.Exists( a_path))
                {
                    //FileSystemWatcher watcher = new FileSystemWatcher();
                    watcher = new FileSystemWatcher();
                    watcher.Path = a_path;
                    // Only watch .fx files
                    watcher.Filter = "*.fx";

                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess
                        | NotifyFilters.FileName | NotifyFilters.DirectoryName
                        | NotifyFilters.Attributes;
                    watcher.EnableRaisingEvents = true;
                    
                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    
                    // If the file is deleted or renamed then delete the old output
                    watcher.Deleted += new FileSystemEventHandler(OnRemoved);
                    watcher.Renamed += new RenamedEventHandler(OnRenamed);

                    m_watchedPaths.Add(a_path, watcher);
                }
            }
        }

        /// <summary>
        /// Get all the watcher paths from arguments
        /// </summary>
        /// <param name="a_arguments"></param>
        /// <returns></returns>
        public string CheckAndCleanArguments(string a_arguments)
        {
            string[] parts = a_arguments.Split(' ');
            
            string output = "";
            bool hasAddedRoot = false;

            for (int i = 1; i < parts.Length; i++)
            {
                string part = parts[i];
                
                if (part != "")
                {
                    // For / add the current folder path not the root for linux!
                    if (part == "/" && !hasAddedRoot)
                    {
                        output = AppContext.BaseDirectory;
                        hasAddedRoot = true;
                    }

                    // On first part don't add a " "
                    if (output == "")
                    {
                        output = part;
                    }
                    // Add the space for all consecutive ones!
                    else
                    {                        
                        output += " " + part;
                    }
                }
            }

            if (output == "" && !hasAddedRoot)
            {
                // Get current working directory
                output = AppContext.BaseDirectory;
                hasAddedRoot = true;
            }

            return output;
        }

        /// <summary>
        /// Set up all watchers based off user input
        /// </summary>
        /// <param name="arguments"></param>
        public void Watch( string a_arguments)
        {
            string[] parts = CheckAndCleanArguments(a_arguments).Split(' ');

            foreach (string part in parts)
            {
                AddWatcher(part);
            }
        }  

        private void OnChanged(object source, FileSystemEventArgs e)
        {            
            m_converter.ConvertFile(e.FullPath);
        }

        // This happens 2 times everytime you save the a file, dunno why but it does!
        private void OnRemoved(object source, FileSystemEventArgs e)
        {          
            if (!File.Exists( e.FullPath ))
            {
                string outputPath = e.FullPath + ".output";
                // Get output file
                if (File.Exists(outputPath))
                {
                    File.Delete(outputPath);
                }
            }                        
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {   
            if (!e.OldName.EndsWith("~"))
            {
                string oldOutput = e.OldFullPath + ".output";
                if ( File.Exists( oldOutput  ))
                {
                    File.Delete(oldOutput);
                }              
            }
            
        }
    }
}
