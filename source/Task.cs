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
        protected IOutputManager outputManager;
        protected InputManager inputManager;

        public Task(InputManager inputManager, IOutputManager outputManager)
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
            Process process = new System.Diagnostics.Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = command;

            process.StartInfo.EnvironmentVariables["Path"] = inputManager.PathEnvironmentVariable;
            process.StartInfo.UseShellExecute = true;  // needed to apply the modified path
                                                       // NOTE:  Maybe "true" causes problems. Look to MSDN for details:
                                                       //        http://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.useshellexecute.aspx

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

            // http://stackoverflow.com/questions/139593/processstartinfo-hanging-on-waitforexit-why
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
                        
            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            if (ConsoleOutputManager.ContainsWarningKeyword(e.Data))
                                // contains warning keyword
                                outputManager.Warning(e.Data);
                            else
                                // normal message
                                outputManager.Info(e.Data);
                        }

                        //-- catch Ogre features --

                        //     NOTE: The information is printed line by line. (not a single message)
                        //           So catch every line (of following outputs) between the 2 following rulers "----------------"

                        // check:  disable logging?
                        if (e.Data.Contains("----------------") 
                            && (outputManager.FeatureSummary.Length > 0))  // ignore fist line of "-" symbols
                        {
                            outputManager.FeatureLoggingIsEnabled = false;
                        }

                        // do logging
                        if (outputManager.FeatureLoggingIsEnabled && (e.Data.Contains("----------------") == false))
                            outputManager.FeatureSummary += e.Data + " \n";

                        // check:  enable logging?
                        if (e.Data.Contains("FEATURE SUMMARY"))
                            outputManager.FeatureLoggingIsEnabled = true;

                        output.AppendLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        if(!string.IsNullOrWhiteSpace(e.Data))
                            outputManager.Error(e.Data);

                        error.AppendLine(e.Data);
                    }
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }

            CommandResult result = new CommandResult(output.ToString(), error.ToString(), process.ExitCode);
            process.Dispose();
            return result;
        }

        private string OutputAndCapture(Process process)
        {
            outputManager.Error(process.StandardError.ReadToEnd());

            string output = process.StandardOutput.ReadToEnd();
            outputManager.Info(output);
            return output;
        }

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