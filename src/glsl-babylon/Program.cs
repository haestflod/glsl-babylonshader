using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace glsl_babylon
{
    public class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public Program(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public static void Main(string[] args)
        {
            var app = new classes.Application();
            app.Run();
        }
    }
}
