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
        
        static void Main(string[] cmdLineArgs)
        {
            Int64 startTime = DateTime.Now.Ticks;

            Console.BufferHeight = 9999;  // keep all messages visble 
            Console.Title = "MogreBuilder";
            
            // enlarge console window width
            Int32 width = 120;  // --> 1000 pixels  (with default settings of Windows 7)
            if (Console.WindowWidth < width)
            {
                // be shure that it fits to the monitor
                if (width > Console.LargestWindowWidth)
                    width = Console.LargestWindowWidth;

                Console.BufferWidth = width;
                Console.WindowWidth = width;
            }

            ConsoleOutputManager outputManager = new ConsoleOutputManager();

            // print information if started from Visual Studio
            PrintIfVisualStudio(cmdLineArgs, outputManager);

            // check if arguments available
            if (cmdLineArgs.Length == 0)
            {
                outputManager.Warning("\nYou need some arguments to run MogreBuilder! ");
                ShowHelp();
            }
            else
            {
                // DO WORK

                Boolean developmentFlag = DevelopmentArgumentCheck(cmdLineArgs);

                if (developmentFlag == true)
                {
                    // --> don't catch exceptions  (better for debugging inside of Visual Studio)
                    outputManager.DisplayMessage("Development mode enabled:  No catching of unexcpected exceptions", ConsoleColor.White);
                    DoWork(cmdLineArgs, outputManager);
                }
                else
                {
                    try
                    {
                        // --> catch exceptions  (good for common users)
                        DoWork(cmdLineArgs, outputManager);
                    }
                    catch (ParseException)
                    {
                        outputManager.Error("Application stopped.");
                    }
                    catch (Exception e)
                    {
                        // print error message
                        outputManager.Error(e.Message);
                        Misc.PrintExceptionTrace(outputManager, e.StackTrace);
                        outputManager.Error("\n BUILD PROCESS ABORTED\n\n.");
                        
                        outputManager.Info(
                            "Please report crashes in our MogreBuilder forum topic.  \n" + 
                            "     A screenshoot of the stack trace and error messages would be nice.\n" );
                    }
                } // else

            } // else



            // print duration time
            PrintDurationTime(startTime, outputManager);

            // highlight window in taskbar
            HighlightInTaskbar(startTime);

            

#if DEBUG
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
#endif
        } // Main()





        private static void DoWork(string[] cmdLineArgs, ConsoleOutputManager outputManager)
        {
            // Properties of successfully assigned arguments
            CommandLineArgs parsedArgs = new CommandLineArgs();

            // Default priority for worker processes.
            parsedArgs.priority = ProcessPriorityClass.BelowNormal;

            ParseCommandLine(cmdLineArgs, outputManager, ref parsedArgs);

            InputManager inputManager = new InputManager(parsedArgs.TargetDir, parsedArgs);
            TaskManager taskManager = new TaskManager(inputManager, outputManager);

            VerifyTargetDirectory(inputManager.TargetDirectory, outputManager);
            VerifyMore(inputManager, outputManager);

            inputManager.GeneratePathVariables();


            // apply priority setting
            if (parsedArgs.priority != ProcessPriorityClass.Normal)
                ProcessPriorityControl.StartController(parsedArgs.priority, outputManager);

            // do tasks
            taskManager.Run();

            // print summary
            outputManager.PrintSummary(inputManager);

        } // DoWork()






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




        /// <summary>
        /// If started by Visual Studio, the arguments will be printed. (Otherwise you can't see it.)
        /// </summary>
        private static void PrintIfVisualStudio(String[] cmdLineArgs, ConsoleOutputManager outputManager)
        {
            // print nothing if started by console or Windows Explorer
            if ((Console.CursorLeft != 0) || (Console.CursorTop != 0))
                return;

            // print directory
            outputManager.DisplayMessage("Current directory:  ", ConsoleColor.Gray);
            outputManager.DisplayMessage(Directory.GetCurrentDirectory(), ConsoleColor.White);

            // print arguments
            outputManager.DisplayMessage("Used arguments:  ", ConsoleColor.Gray);

            String all = "";
            foreach (String arg in cmdLineArgs)
            {
                String argPrint = arg;

                // re-add quotation for arguments with spaces 
                //  --> display arguments equal to the shell input
                if (argPrint.Contains(" "))
                    argPrint = "\"" + argPrint + "\"";

                all += argPrint + " ";
            }
            outputManager.DisplayMessage(all + "\n", ConsoleColor.White);
        }


    } // class Program

} // namespace
