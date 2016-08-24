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
                    FileSystemWatcher watcher = new FileSystemWatcher();
                    watcher.Path = a_path;
                    // Only watch .fx files
                    watcher.Filter = "*.fx";

                    watcher.NotifyFilter = NotifyFilters.LastWrite;
                        //| NotifyFilters.FileName | NotifyFilters.DirectoryName;

                    watcher.Created += new FileSystemEventHandler(OnChanged);
                    watcher.Changed += new FileSystemEventHandler(OnChanged);
                    
                    // If the file is deleted or renamed then delete the old output
                    watcher.Deleted += new FileSystemEventHandler(OnRemoved);
                    watcher.Renamed += new RenamedEventHandler(OnRemoved);

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

            for (int i = 1; i < parts.Length; i++)
            {
                string part = parts[i];
                
                if (part != "")
                {
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

            if (output == "")
            {
                // Get current working directory
                output = AppContext.BaseDirectory;
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
            Console.WriteLine("Changed:");
            Console.WriteLine(e.FullPath);
        }

        private void OnRemoved(object source, FileSystemEventArgs e)
        {
            Console.WriteLine("Removed:");
            Console.WriteLine(e.FullPath);
        }

        private void OnRenamed(object source, RenamedEventHandler e)
        {
            Console.WriteLine("Renamed:");
            Console.WriteLine();
        }
    }
}
