using System;
using System.IO;
using System.Collections.Generic;
using Mogre.Builder.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Mogre.Builder
{
    partial class Program
    {
        
        struct CommandLineArgs
        {
            public string TargetDir { get; set; }
            public string ConfigFile { get; set; }
            public ProcessPriorityClass priority { get; set; }
        }



        static void Main(string[] cmdLineArgs)
        {
            Int64 startTime = DateTime.Now.Ticks;
            var outputManager = new ConsoleOutputManager();

            try
            {
                if (cmdLineArgs.Length == 0)
                    ShowHelp();
                else
                {
                    // Properties of successfully assigned arguments
                    CommandLineArgs parsedArgs = new CommandLineArgs();

                    // Default priority for worker processes.
                    parsedArgs.priority = ProcessPriorityClass.BelowNormal;


                    ParseCommandLine(cmdLineArgs, outputManager, ref parsedArgs);
                    var inputManager = new InputManager(parsedArgs.TargetDir, parsedArgs.ConfigFile);
                    var taskManager = new TaskManager(inputManager, outputManager);

                    VerifyTargetDirectory(inputManager.TargetDirectory, outputManager);

                    // apply priority setting
                    if (parsedArgs.priority != ProcessPriorityClass.Normal)
                        ProcessPriorityControl.StartController(parsedArgs.priority, outputManager);

                    taskManager.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            PrintDurationTime(startTime, outputManager);

#if DEBUG
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
#endif
        } // Main()





        private static void ShowHelp()
        {
            Console.WriteLine("You have to define the target directory as first argument! \n\n");
            
            // TODO:  Write help file and load it here.

        }





        private static void PrintDurationTime(Int64 startTime, ConsoleOutputManager outputManager)
        {
            DateTime duration = new DateTime(DateTime.Now.Ticks - startTime);
            String hourString = "hours";
            String minuteString = "minutes";
            if (duration.Hour == 1)
                hourString = "hour"; // singular
            if (duration.Minute == 1)
                minuteString = "minute";

            outputManager.DisplayMessage(String.Format(
                "\nDuration:  {0} {1} {2} {3} ", duration.Hour, hourString, duration.Minute, minuteString),
                ConsoleColor.White);
        } // PrintDurationTime()



    } // class Program

} // namespace
