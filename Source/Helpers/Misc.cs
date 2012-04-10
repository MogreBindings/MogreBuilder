using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Mogre.Builder
{
    class Misc
    {
        /// <summary>
        /// Make paths bullet-proof for usage with Mercurial (HG). 
        /// This is important when a directory contains a space symbol.
        /// </summary>
        /// <param name="path">Input path</param>
        /// <returns>bullet-proof path</returns>
        public static String HgPathSecurity(String path)
        {
            // check for spaces in source path  (can happen for a clone on local hard disk)
            if (Regex.IsMatch(path, " "))
            {
                // remove backslash from end 
                // because it causes a conflict with the following quote symbol (Mercurial assumes that \" is an escaped quote)
                path = path.TrimEnd('\\');

                // add quote symbols
                path = "\"" + path + "\"";

                //Console.WriteLine("MODIFIED HG PATH:  " + path);
            }
            return path;
        } // HgPathSecurity()





        /// <summary>
        /// Remove a specific application entry from the environment variable "PATH". 
        /// The settings will only applied to worker processes. 
        /// </summary>
        public static void ModifyEnvironmentPath(ref CommandLineArgs parsedArgs, ConsoleOutputManager outputManager, PathRemove whatToRemove)
        {

            // read all directories
            List<String> directoryList = new List<String>(Regex.Split(parsedArgs.PathEnvironmentVariable, ";"));

            List<String> killingList = new List<String>();

            // seach for directories, which to remove   (also multiple entries will be removed)
            foreach (String directory in directoryList)
            {
                switch (whatToRemove)
                {
                    case PathRemove.Boost:
                        if (Regex.IsMatch(directory, "boost", RegexOptions.IgnoreCase))
                            killingList.Add(directory);
                        break;

                    // test case to check if it works
                    case PathRemove.Test:
                        if (Regex.IsMatch(directory, @"MogreBuilderTEST", RegexOptions.IgnoreCase))
                            killingList.Add(directory);
                        break;

                        
                    default:
                        outputManager.Warning(String.Format(
                            "Can't remove '{0}' entry from PATH environment variable. \n" +
                            "      Note for developers:  Add a case statement to ModifyEnvironmentPath() ", whatToRemove));
                        break;
                }
            }

            // create content of modified PATH variable
            String newPath = "";
            foreach (String directory in directoryList)
                if (killingList.Contains(directory) == false)
                    newPath += directory + ";";

            // save the changes
            parsedArgs.PathEnvironmentVariable = newPath;

            // console output
            foreach (String removed in killingList)
                outputManager.Info("Ignore this directory of PATH environment variable: \n    " + removed);

        } // ModifyEnvironmentPath()



        public enum PathRemove
        {
            Test,
            Boost
        }




        public static void PrintExceptionTrace(IOutputManager outputManager, String trace)
        {
            if ((trace == null) || (trace == ""))
                return;

            String message = 
                "\nException stack trace for debugging: \n" +
                "--------------------------------------------------------------------------\n" +
                trace +
                "--------------------------------------------------------------------------\n\n";
            outputManager.Info(message);

        } // PrintExceptionTrace()


    } // class Misc
}
