﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

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

            if (arguments != null)
                process.StartInfo.Arguments = arguments;

            if (workingDirectory != null)
            {
                if(!Directory.Exists(workingDirectory))
                    throw new Exception(string.Format("{0} does not exist", workingDirectory));

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
            process.Start();

            string output = "";
            string standardOutput;
            while (!process.WaitForExit(500))
            {
                standardOutput = process.StandardOutput.ReadToEnd();
                outputManager.Info(standardOutput);
                output += standardOutput;
                outputManager.Progress();
            }

            standardOutput = process.StandardOutput.ReadToEnd();
            outputManager.Info(standardOutput);
            output += standardOutput;

            string error = process.StandardError.ReadToEnd();
            var result = new CommandResult(output, error, process.ExitCode);
            process.Dispose();
            return result;
        }

        protected void ModifyFile(string filePath, string pattern, string replacement)
        {
            string text = File.ReadAllText(filePath);
            string modifiedText = Regex.Replace(text, pattern, replacement);

            File.WriteAllText(filePath, modifiedText);
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