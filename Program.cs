using System;
using System.IO;
using System.Collections.Generic;
using Mogre.Builder.Tasks;

namespace Mogre.Builder
{
    class Program
    {
        struct CommandLineArgs
        {
            public string TargetDir { get; set; }
            public string ConfigFile { get; set; }
        }

        static void Main(string[] cmdLineArgs)
        {
            try
            {
                var parsedArgs = ParseCommandLine(cmdLineArgs);
                var inputManager = new InputManager(parsedArgs.TargetDir, parsedArgs.ConfigFile);
                var outputManager = new ConsoleOutputManager();
                var taskManager = new TaskManager(inputManager, outputManager);
                
                taskManager.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

#if DEBUG
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
#endif
        }

        private static CommandLineArgs ParseCommandLine(string[] cmdLineArgs)
        {
            var parsedArgs = new CommandLineArgs();

            if (cmdLineArgs.Length > 0)
                parsedArgs.TargetDir = cmdLineArgs[0];

            if(cmdLineArgs.Length == 3)
            {
                if (cmdLineArgs[1] == "-config")
                    parsedArgs.ConfigFile = cmdLineArgs[2];
            }

            return parsedArgs;
        }
    }


}
