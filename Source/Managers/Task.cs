using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace Mogre.Builder
{
    abstract class Task
    {
        protected OutputManager outputManager;
        protected InputManager inputManager;

        public Task(InputManager inputManager, OutputManager outputManager)
        {
            this.outputManager = outputManager;
            this.inputManager = inputManager;
        }

        abstract public void Run();

        abstract public string ID          { get; }
        abstract public string Name        { get; }
        abstract public string Description { get; }

        protected CommandResult RunCommand(string command, string arguments, string workingDirectory)
        {
            Process process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = command;

            process.StartInfo.EnvironmentVariables["Path"] = inputManager.PathEnvironmentVariable;

            if (arguments != null)
                process.StartInfo.Arguments = arguments;

            if (workingDirectory != null)
            {
                if(!Directory.Exists(workingDirectory))
                    throw new Exception(String.Format("{0} does not exist", workingDirectory));

                if (Path.IsPathRooted(workingDirectory))
                    process.StartInfo.WorkingDirectory = workingDirectory;
                else
                    process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\" + workingDirectory;
            }
            else
            {
                process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            }
            
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
                        
            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                    return;

                if (!string.IsNullOrWhiteSpace(e.Data))
                {
                    //-- console output  (optionally as warning) --

                    if (OutputManager.ContainsWarningKeyword(e.Data))
                        // contains warning keyword
                        outputManager.Warning(e.Data);
                    else
                        // normal message
                        outputManager.Info(e.Data);


                    //-- catch Ogre features --

                    //     NOTE: The information is printed line by line. (not a single message)
                    //           So catch every line (of following outputs) between the 2 following rulers "----------------"

                    // check:  disable logging?
                    if (e.Data.Contains("----------------")
                        && (outputManager.FeatureSummary.Length > 0))  // ignore fist line of "-" symbols
                    {
                        outputManager.IsFeatureLoggingEnabled = false;
                    }

                    // do logging
                    if (outputManager.IsFeatureLoggingEnabled && (e.Data.Contains("----------------") == false))
                        outputManager.FeatureSummary += e.Data + " \n";

                    // check:  enable logging?
                    if (e.Data.Contains("FEATURE SUMMARY"))
                        outputManager.IsFeatureLoggingEnabled = true;

                }
                output.AppendLine(e.Data);
            };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                    return;

                if (!string.IsNullOrWhiteSpace(e.Data))
                    outputManager.Error(e.Data);

                error.AppendLine(e.Data);
            };
        
                

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
                
            process.WaitForExit();

            CommandResult result = new CommandResult(output.ToString(), error.ToString(), process.ExitCode);
            process.Dispose();
            return result;
        }


        /// <summary>
        /// Open a file and replace strings. The replacement is based on regular expressions. <br/>
        /// IMPORTANT: With 3 arguments the RegEx scope is only one SINGLE LINE. (The RegEx call will be applied line by line.) 
        /// With 4 arguments the regular expression scope is the WHOLE FILE. 
        /// </summary>
        /// <param name="filePath">Path of the file</param>
        /// <param name="pattern">Regular expression pattern (No pure String search!)</param>
        /// <param name="replacement">String to replace the matches</param>
        protected void ModifyFile(string filePath, string pattern, string replacement)
        {
            List<string> lineList = new List<string>();

            using (StreamReader reader = new StreamReader(filePath))
            {                
                string line = reader.ReadLine();

                while (line != null)
                {
                    lineList.Add(Regex.Replace(line, pattern, replacement));
                    line = reader.ReadLine();
                }

                reader.Close();
            }

            using(StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string line in lineList)
                    writer.WriteLine(line);

                writer.Close();
            }
        }



        /// <summary>
        /// Open a file and replace strings. The replacement is based on regular expressions. <br/>
        /// IMPORTANT:  <br/>
        /// With 3 arguments the RegEx scope is only one SINGLE LINE. (The RegEx call will be applied line by line.)  <br/>
        /// With 4 arguments the regular expression scope is the WHOLE FILE. 
        /// <param name="filePath">Path of the file</param>
        /// <param name="pattern">Regular expression pattern (No pure String search!)</param>
        /// <param name="replacement">String to replace the matches</param>
        protected void ModifyFile(string filePath, string pattern, string replacement, RegexOptions regexOptions)
        {
            string text = File.ReadAllText(filePath);
            string modifiedText = Regex.Replace(text, pattern, replacement, regexOptions);

            File.WriteAllText(filePath, modifiedText);
        }
    }



    struct CommandResult
    {
        public string Output;
        public string Error;
        public int ExitCode;

        public CommandResult(string output, string error, int exitCode)
        {
            Output   = output;
            Error    = error;
            ExitCode = exitCode;
        }
    }
}