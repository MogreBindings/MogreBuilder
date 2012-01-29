using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Mogre.Builder
{
    abstract class Task
    {
        protected IOutputManager outputManager;
        
        public Task(IOutputManager outputManager)
        {
            this.outputManager = outputManager;
        }

        abstract public void Run();

        abstract public string ID          { get; }
        abstract public string Name        { get; }
        abstract public string Description { get; }

        //protected CommandResult Cmd(string cmd)
        //{
        //    return Cmd(cmd, null);
        //}

        protected CommandResult Cmd(string command, string arguments, string workingDirectory)
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
            
            string output = "";
            process.OutputDataReceived += delegate(object sender, System.Diagnostics.DataReceivedEventArgs e)
            {
                output += e.Data + "\n";
            };

            string error = "";
            process.ErrorDataReceived += delegate(object sender, System.Diagnostics.DataReceivedEventArgs e)
            {
                error += e.Data + "\n";
            };

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            var result = new CommandResult(output, error, process.ExitCode);
            process.Dispose();
            return result;
        }

        protected void ModifyFile(string file, string pattern, string replacement)
        {
            File.WriteAllText(
                file,
                Regex.Replace(
                    File.ReadAllText(file),
                    pattern,
                    replacement)
            );
        }

        protected void ModifyFile(string file, string pattern, string replacement, RegexOptions regexOptions)
        {
            File.WriteAllText(
                file,
                Regex.Replace(
                    File.ReadAllText(file),
                    pattern,
                    replacement,
                    regexOptions)
            );
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