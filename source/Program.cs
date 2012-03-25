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
            Console.BufferHeight = 9999;  // keep all messages visble 
            Console.Title = "MogreBuilder";
            ConsoleOutputManager outputManager = new ConsoleOutputManager();


            try
            {
                if (cmdLineArgs.Length == 0)
                {
                    outputManager.Warning("\nYou need some arguments to run MogreBuilder! ");
                    ShowHelp();
                }
                else
                {
                    // Properties of successfully assigned arguments
                    CommandLineArgs parsedArgs = new CommandLineArgs();

                    // Default priority for worker processes.
                    parsedArgs.priority = ProcessPriorityClass.BelowNormal;

                    ParseCommandLine(cmdLineArgs, outputManager, ref parsedArgs);
                    InputManager inputManager = new InputManager(parsedArgs.TargetDir, parsedArgs.ConfigFile);
                    TaskManager taskManager = new TaskManager(inputManager, outputManager);

                    VerifyTargetDirectory(inputManager.TargetDirectory, outputManager);
                    VerifyMore(inputManager, outputManager);

                    // apply priority setting
                    if (parsedArgs.priority != ProcessPriorityClass.Normal)
                        ProcessPriorityControl.StartController(parsedArgs.priority, outputManager);

                    // do tasks
                    taskManager.Run();

                    // print summary
                    outputManager.PrintSummary();

                    
                }
            }
            catch (Exception ex)
            {
                outputManager.Error(ex.Message);
            }


            // print duration time
            PrintDurationTime(startTime, outputManager);

            // highlight window in taskbar
            HighlightInTaskbar(startTime);

            

#if DEBUG
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
#endif
        } // Main()





        private static void ShowHelp()
        {
            Console.WriteLine("Define the target directory as first argument! \n\n");

            // TODO:  Write help file and load it here.

        }





        private static void PrintDurationTime(Int64 startTime, ConsoleOutputManager outputManager)
        {
            TimeSpan duration = TimeSpan.FromTicks((DateTime.Now.Ticks - startTime));

            String hourString = "hours";
            String minuteString = "minutes";
            if (duration.Hours == 1)
                hourString = "hour"; // singular
            if (duration.Minutes == 1)
                minuteString = "minute";

            outputManager.DisplayMessage(String.Format(
                "\nDuration:  {0} {1} {2} {3} ", duration.Hours, hourString, duration.Minutes, minuteString),
                ConsoleColor.White);
        } // PrintDurationTime()





        /// <summary>
        /// Highlight the window in the taskbar. 
        /// When window is in background, blink until it gets the focus. 
        /// When window is in foreground, highlight it forever without blinking.
        /// When the application was just started, do nothing.
        /// </summary>
        /// <param name="startTime">Start time of the application</param>
        private static void HighlightInTaskbar(Int64 startTime)
        {
            TimeSpan duration = TimeSpan.FromTicks((DateTime.Now.Ticks - startTime));

            // do nothing if just started
            if (duration.TotalSeconds > 10)
            {
                if (TaskbarHighlight.WindowIsInForeground())
                    TaskbarHighlight.StaticHighlight();
                else
                    TaskbarHighlight.BlinkUntilFocus();
            }

        } // HighlightInTaskbar()


    } // class Program

} // namespace
