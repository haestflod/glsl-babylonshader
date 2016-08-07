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
            ConvertFile,
            ConvertFolder,
            WatchFolder               
        }

        private Action SelectedAction { get; set; }
        private string ActionValue { get; set; }

        private Converter m_converter;

        public Application()
        {
            m_converter = new Converter();
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
                ActionValue = Console.ReadLine();

                string[] parts = ActionValue.Split(' ');

                switch (parts[0])
                {
                    case ExitAction:
                        SelectedAction = Action.Exit;
                        break;
                    case ConvertAction:
                        SelectedAction = Action.ConvertFile;
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
                case Action.ConvertFile:
                    m_converter.Convert(ActionValue);
                    break;
                case Action.ConvertFolder:
                    break;
                case Action.WatchFolder:
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
