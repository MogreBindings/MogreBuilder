using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Mogre.Builder
{

    partial class Program
    {

        /// <summary>
        /// Process all input arguments from the command line. 
        /// The first argument is assumed to be the target pathVar. (Optional, because maybe in the future it can also be defined by a config file). 
        /// For all other arguments it will tried to associate the wanted option. 
        /// </summary>
        private static CommandLineArgs ParseCommandLine(String[] cmdLineArgs, ConsoleOutputManager outputManager, ref CommandLineArgs parsedArgs)
        {
            // The inputList contains all arguments. Successfully parsed arguments will be removed.
            List<String> inputList = new List<String>(cmdLineArgs); 


            // check if first argument could be a pathVar
            if ((inputList.Count > 0) && (inputList[0].StartsWith("-") == false))
            {
                String directory = inputList[0];
                inputList.RemoveAt(0);

                directory = Regex.Replace(directory, "\"", ""); // remove quotes
                parsedArgs.TargetDir = directory; // save
            }


            // check for config file option
            parsedArgs.ConfigFile = ReadValue("-config", inputList);
            // TODO: Check if file exists


            // check if user wants to compile with VS 2012
            parsedArgs.Vs2012 = ReadBool("-vs2012", inputList);


            // check if user wants to compile with VS 2012
            parsedArgs.X64 = ReadBool("-x64", inputList);


            // check for process priority option
            string priority = ReadValue("-priority", inputList);
            if(priority != null)
            {
                try
                {
                    parsedArgs.priority =
                        (ProcessPriorityClass) Enum.Parse(typeof (ProcessPriorityClass), priority, true);
                }
                catch(ArgumentException)
                {
                    outputManager.Warning(String.Format(
                        "WARNING:  Unknown priority parameter '{0}' \n" +
                        "          Useful values: 'Idle', 'BelowNormal', 'AboveNormal'  (not case-sensitive) ",
                        priority));
                }
            }


            // check if user wants to disable boost
            if(ReadBool("-noboost", inputList))
            {
                Misc.ModifyEnvironmentPath(ref parsedArgs, outputManager, Misc.PathRemove.Boost);
                Misc.ModifyEnvironmentPath(ref parsedArgs, outputManager, Misc.PathRemove.Test);  // TEST
            }


            // check if user wants to skip the CMake processing 
            parsedArgs.SkipCMake = ReadBool("-skipcmake", inputList);


            // check if user doesn't want to update repositories. 
            parsedArgs.NoUpdate = ReadBool("-noupdate", inputList);


            // check if user wants to disable the catching of unspecific exceptions
            parsedArgs.DevelopmentFlag = ReadBool("-development", inputList);


            // check if user wants to enable MogreNewt
            parsedArgs.MogreNewt = ReadBool("-mogrenewt", inputList);


            // check if user wants to enable MOIS
            parsedArgs.Mois = ReadBool("-mois", inputList);


            // check if user wants to skip the Mogre/Ogre build 
            parsedArgs.OnlyAddons = ReadBool("-onlyaddons", inputList);


            //------------------>>>>>>> ... more option here ...  <<<<<<<<------------------
            

            //--- UNKNOWN ARGUMENTS ---

            // show a warning for all arguments, which are not processed
            if (inputList.Count > 0)
            {
                outputManager.Warning(
                    "\nWARNING \n" + 
                    "Can't assign these arguments:  ");

                // display recent arguments
                String unknownArguments = "";
                foreach (String unknown in inputList)
                    unknownArguments += unknown + " ";
                outputManager.DisplayMessage(unknownArguments + "\n", ConsoleColor.White);

                outputManager.DisplayMessage(
                    "Perhaps your target path has spaces? \n" +
                    "In this case use quotes. \n", ConsoleColor.Yellow);

                // ask user if he want to quit
                outputManager.DisplayMessage(
                    "Do you want to continue?  [y/n]   ", ConsoleColor.Cyan);
                Console.WriteLine();

                ConsoleKeyInfo answer = Console.ReadKey(true);
                if (answer.Key.ToString().ToLower() != "y")
                {
                    // user want to abort
                    outputManager.Warning(
                        "Note: Call MogreBuilder without argument to see the help.");
                    throw new ParseException(); // stop application
                }
                
            } // if


            return parsedArgs;
        }



        /// <summary>
        /// Read a boolean option from the argument list. If found, the option is removed from list.
        /// </summary>
        /// <param name="option">Option to read.</param>
        /// <param name="inputList">List of command line arguments</param>
        /// <returns>True if option was set. Otherwise false.</returns>
        private static bool ReadBool(string option, List<string> inputList)
        {
            for (Int16 i = 0; i < inputList.Count; i++)
            {
                if (inputList[i].ToLower() == option)
                {
                    inputList.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Read a value from the argument list. Values are specified on the command line as "-option value".
        /// If found, the option and the value are removed from list.
        /// </summary>
        /// <param name="option">Option to read.</param>
        /// <param name="inputList">List of command line arguments</param>
        /// <returns>Value if it was set. Otherwise null.</returns>
        private static string ReadValue(string option, List<string> inputList)
        {
            for (Int16 i = 0; i < inputList.Count; i++)
            {
                if (inputList[i].ToLower() == option)
                {
                    string value = inputList[i + 1];
                    inputList.RemoveRange(i, 2);
                    return value;
                }
            }

            return null;
        }

// ParseCommandLine()





        /// <summary>
        /// Check if target directory exists. If not, it will tried to create it. 
        /// Messages should help the user with problems. 
        /// </summary>
        /// <param name="pathVar">pathVar of target directory</param>
        /// <param name="outputManager">Used to created coloured console outputs.</param>
        private static void VerifyTargetDirectory(String path, ConsoleOutputManager outputManager)
        {
            if (path == null)
                path = "";

            // check if exists
            if (Directory.Exists(path) == false)
            {
                // check if seems to be a valid pathVar parameter
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
                        throw new ParseException(); // stop application
                    }
                }
                else
                {
                    // show info if pathVar arguments seems to be invalid
                    outputManager.Warning(String.Format(
                        "NOTE: \n" + 
                        "The first argument has to be the path of the target directory. \n\n" +
                        "Either use an absolute path (then the directory will be created automatically) \n" + 
                        "OR create the directory manually if you want to use a relative path. \n"
                        ));
                    outputManager.Error(String.Format(
                        "Aborted, because the path argument seems to be invalide. \n" +
                        "Path argument: '{0}'", path));

                    throw new ParseException(); // stop application
                }
            }
        } // VerifyTargetDirectory()





        private static void VerifyMore(InputManager inputManager, ConsoleOutputManager outputManager)
        {

//            // Try to find in environment variable "pathVar"
//
//            String pathVar = System.Environment.GetEnvironmentVariable("path");
//            //String[] pathList = Regex.Split(pathVar, ";");
//
//            String[] pathList = {"c:\\ddd", "c:\\ddd\\", "b:\\bb"}; // TEST
//
//            foreach (String path in pathList)
//            {
//                String fullPath = Path.Combine(path, "cmake.exe");  // with Combine() it doesn't care if the path has a backslash at the end
//                Console.WriteLine(fullPath);
//
//                //if (File.Exists())
//            }
//            throw new NotImplementedException("CONTINUE HERE");
//            return; // TEST



            if (File.Exists(inputManager.CMakeExecutable) == false)
            {
                outputManager.Error(String.Format(
                    "Aborted, because the CMake path seems to be wrong! \n" +
                    "Currently it's: '{0}' \n" + 
                    "Enter the correct path into your config file.", inputManager.CMakeExecutable));

                throw new Exception("Application stopped");  // force an exit


            }


        } // VerifyMore()





        /// <summary>
        /// Check if the argument "-development" was used.
        /// </summary>
        private static Boolean DevelopmentArgumentCheck(String[] cmdLineArgs)
        {
            foreach (String arg in cmdLineArgs)
                if (arg.ToLower() == "-development")
                    return true;

            return false;
        }


    } // class Program



    public struct CommandLineArgs
    {
        public string TargetDir { get; set; }
        public string ConfigFile { get; set; }
        public Boolean Vs2012 { get; set; }
        public Boolean X64 { get; set; }
        public ProcessPriorityClass priority { get; set; }
        public Boolean MogreNewt { get; set; }
        public Boolean Mois { get; set; }
        public Boolean OnlyAddons { get; set; }
        public Boolean DevelopmentFlag { get; set; }
        public Boolean SkipCMake { get; set; }
        public Boolean NoUpdate { get; set; }

        public String PathEnvironmentVariable 
        {
            get 
            { 
                if (pathEnvironmentVariable == null) 
                    pathEnvironmentVariable = Environment.GetEnvironmentVariable("path");  // system default
                return pathEnvironmentVariable;
            }
            set { pathEnvironmentVariable = value; }
        }

        private String pathEnvironmentVariable;

    } // struct CommandLineArgs


} // namespace
