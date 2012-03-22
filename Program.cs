using System;
using System.IO;
using System.Collections.Generic;
using Mogre.Builder.Tasks;
using System.Text.RegularExpressions;

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
            Int64 startTime = DateTime.Now.Ticks;
            try
            {
                if (cmdLineArgs.Length == 0)
                    ShowHelp();
                else
                {

                    var parsedArgs = ParseCommandLine(cmdLineArgs);
                    var inputManager = new InputManager(parsedArgs.TargetDir, parsedArgs.ConfigFile);
                    var outputManager = new ConsoleOutputManager();
                    var taskManager = new TaskManager(inputManager, outputManager);

                    VerifyTargetDirectory(inputManager.TargetDirectory, outputManager);

                    taskManager.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            PrintDurationTime(startTime);

#if DEBUG
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
#endif
        } // Main()





        /// <summary>
        /// Process all input arguments from the command line. 
        /// The first argument is assumed to be the target path. (Optional, because maybe in the future it can also be defined by a config file). 
        /// For all other arguments it will tried to associate the wanted option. 
        /// </summary>
        private static CommandLineArgs ParseCommandLine(String[] cmdLineArgs)
        {
            // The inputList contains all arguments. Successfully parsed arguments will be removed.
            List<String> inputList = new List<String>(cmdLineArgs); 

            // Properties of successfully assigned arguments
            CommandLineArgs parsedArgs = new CommandLineArgs();
            
            // check if first argument could be a PATH
            if ((inputList.Count > 0) && (inputList[0].StartsWith("-") == false))
            {
                String directory = inputList[0];
                inputList.RemoveAt(0);

                directory = Regex.Replace(directory, "\"", ""); // remove quotes
                parsedArgs.TargetDir = directory; // save
            }

            // check for CONFIG FILE option
            for (Int16 i = 0;   i < inputList.Count - 1;   i++)
            {
                if (inputList[i] == "-config")
                {
                    parsedArgs.ConfigFile = inputList[i + 1];
                    inputList.RemoveRange(i, 2);
                    break;
                }
            }

            // ... more option here ...

            // check if all arguments are processed
            if (inputList.Count > 0)
            {
                ConsoleOutputManager.DisplayMessage(
                    "\nWARNING \n" + 
                    "Can't assign these arguments:  ", ConsoleColor.Yellow);

                // display recent arguments
                String unknownArguments = "";
                foreach (String unknown in inputList)
                    unknownArguments += unknown + " ";
                ConsoleOutputManager.DisplayMessage(unknownArguments + "\n", ConsoleColor.White);

#if DEBUG
                // quote all arguments
                // --> Useful if you run the MogreBuilder by Visual Studio (Then you don't see the arguments.)
                ConsoleOutputManager.DisplayMessage(
                    "Just to remember - Here are all arguments which you used: ", ConsoleColor.Gray);

                String all = "";
                foreach (String arg in cmdLineArgs)
                    all += arg + " ";
                ConsoleOutputManager.DisplayMessage(all + "\n", ConsoleColor.White);
#endif

                ConsoleOutputManager.DisplayMessage(
                    "Perhaps your target path has spaces? \n" +
                    "In this case use quotes. \n", ConsoleColor.Yellow);

                // ask user if he want to quit
                ConsoleOutputManager.DisplayMessage(
                    "Do you want to continue?  [y/n]   ", ConsoleColor.Cyan);
                Console.WriteLine();

                ConsoleKeyInfo answer = Console.ReadKey(true);
                if (answer.Key.ToString().ToLower() != "y")
                {
                    // user want to abort
                    ConsoleOutputManager.DisplayMessage(
                        "Note: Call MogreBuilder without argument to see the help.", ConsoleColor.Yellow);
                    throw new Exception("Application stopped");
                }
                
            }

            return parsedArgs;
        } // ParseCommandLine()





        private static void ShowHelp()
        {
            Console.WriteLine("You have to define the target directory as first argument! \n\n");
            
            // TODO:  Write help file and load it here.

        }





        /// <summary>
        /// Check if target directory exists. If not, it will tried to create it. 
        /// Messages should help the user with problems. 
        /// </summary>
        /// <param name="path">path of target directory</param>
        /// <param name="outputManager">Used to created coloured console outputs.</param>
        private static void VerifyTargetDirectory(String path, ConsoleOutputManager outputManager)
        {
            // check if exists
            if (Directory.Exists(path) == false)
            {
                // check if seems to be a valid path parameter
                if (Regex.IsMatch(path, @"^[A-Za-z]:\\."))  // true e.g. for "C:\myTarget"
                {
                    outputManager.Action(String.Format(
                        "Create target directory:  {0} ", path));

                    // create directory
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception e)
                    {
                        outputManager.Error(String.Format(
                            "FAILED to create directory. \n" + 
                            "Error message:  {0} ", e.Message));
                        throw new Exception("Application stopped");  // force an exit
                    }
                }
                else
                {
                    // show info if path arguments seems to be invalid
                    outputManager.Warning(String.Format(
                        "NOTE: \n" + 
                        "The first argument has to be the path of the target directory. \n\n" +
                        "Either use an absolute path (then the directory will be created automatically) \n"  + 
                        "OR create the directory manually if you want to use a relative path. \n"
                        ));
                    outputManager.Error(String.Format(
                        "Aborted, because the path argument seems to be invalide. \n" +
                        "Path argument: '{0}'", path));
                    
                    throw new Exception("Application stopped");  // force an exit
                }
            }
        } // VerifyTargetDirectory()





        private static void PrintDurationTime(Int64 startTime)
        {
            DateTime duration = new DateTime(DateTime.Now.Ticks - startTime);
            String hourString = "hours";
            String minuteString = "minutes";
            if (duration.Hour == 1)
                hourString = "hour"; // singular
            if (duration.Minute == 1)
                minuteString = "minute";

            ConsoleOutputManager.DisplayMessage(String.Format(
                "\nDuration:  {0} {1} {2} {3} ", duration.Hour, hourString, duration.Minute, minuteString),
                ConsoleColor.White);
        } // PrintDurationTime()



    } // class Program

} // namespace
