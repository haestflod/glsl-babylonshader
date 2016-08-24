using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace glsl_babylon.classes
{
    public class Application
    {
        public const string ConvertAction = "c";
        public const string WatchAction = "w";
        public const string ExitAction = "x"; 

        enum Action
        {
            None,
            Exit,
            Convert,            
            Watch               
        }

        private Action SelectedAction { get; set; }
        /// <summary>
        /// The selected arguments when choosing an action.
        /// Also includes the action itself
        /// </summary>
        private string Arguments { get; set; }

        private IConverter m_converter;
        private Watcher m_watcher;

        public Application()
        {
            m_converter = new Converter();
            m_watcher = new Watcher(m_converter);
        }

        // For Dependancy injection!
        public Application( IConverter a_converter, Watcher a_watcher)
        {
            m_converter = a_converter;
            m_watcher = a_watcher;
        }

        public void Run()
        {
            Console.WriteLine("Welcome to the glsl to babylon shaderstore converter.");
            Console.WriteLine("When converting or watching a folder the relative path starts where the program is.");
            Console.WriteLine("When converting you can either use a filename or a foldername.");
            Console.WriteLine("For example: 'c file folder' will convert file.vertex.fx, file.fragment.fx and all files in folder");

            while (SelectedAction != Action.Exit)
            {
                GetUserAction();
                DoAction();
            }
        }

        public void GetUserAction()
        {
            SelectedAction = Action.None;            
            while (SelectedAction == Action.None)
            {
                Console.WriteLine();
                Console.WriteLine("Available actions");
                Print( String.Format( "'{0} [name]'", ConvertAction), ConsoleColor.Cyan);           
                Console.WriteLine(" - One time conversion" );
                Print(String.Format("'{0} [foldername]'", WatchAction), ConsoleColor.Magenta);
                Console.WriteLine(" - Watches a folder for changes");
                Print(String.Format("'{0}'", ExitAction), ConsoleColor.Yellow);
                Console.WriteLine(" - Exit");
                

                Console.Write("Action: ");
                Arguments = Console.ReadLine();

                // Get the action part
                string[] parts = Arguments.Split(' ');

                switch (parts[0])
                {
                    case ExitAction:
                        SelectedAction = Action.Exit;
                        break;
                    case ConvertAction:
                        SelectedAction = Action.Convert;
                        break;
                    case WatchAction:
                        SelectedAction = Action.Watch;
                        break;
                    default:
                        Console.WriteLine("No such action exists");
                        break;
                }
            }
        }

        public void DoAction()
        {
            switch (SelectedAction)  
            {
                case Action.Convert:
                    m_converter.Convert(Arguments);
                    break;
                case Action.Watch:
                    m_watcher.Watch(Arguments);
                    break;
            }
        }

        public static void Print(string a_text, ConsoleColor a_color)
        {
            ConsoleColor origColor = Console.ForegroundColor;
            Console.ForegroundColor = a_color;
            Console.Write(a_text);
            Console.ForegroundColor = origColor;
        }

        /// <summary>
        /// Print a console line with a specific color
        /// </summary>
        /// <param name="a_line"></param>
        /// <param name="a_color"></param>
        public static void PrintLine(string a_line, ConsoleColor a_color)
        {
            ConsoleColor origColor = Console.ForegroundColor;
            Console.ForegroundColor = a_color;
            Console.WriteLine(a_line);
            Console.ForegroundColor = origColor;
        }
    }
}
