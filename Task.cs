using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Mogre.Builder
{
    abstract class Task
    {
        protected OutputManager outputMgr;

        
        
        public Task(OutputManager outputMgr)
        {
            this.outputMgr = outputMgr;
        }

        abstract public void Run();

        abstract public string ID          { get; }
        abstract public string Name        { get; }
        abstract public string Description { get; }

        protected CommandResult Cmd(string cmd)
        {
            return Cmd(cmd, null);
        }

        protected CommandResult Cmd(string cmd, string path)
        {
            var parts = cmd.Split(new char[] { ' ' }, 2);

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = parts[0];
            if (parts.Length > 1)
                proc.StartInfo.Arguments = parts[1];
            if (path != null)
            {
                if (Path.IsPathRooted(path))
                    proc.StartInfo.WorkingDirectory = path;
                else
                    proc.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory() + "\\" + path;
            }
            else
            {
                proc.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            }


            string output = "";
            proc.OutputDataReceived += delegate(object sender, System.Diagnostics.DataReceivedEventArgs e)
            {
                output += e.Data + "\n";
            };

            string error = "";
            proc.ErrorDataReceived += delegate(object sender, System.Diagnostics.DataReceivedEventArgs e)
            {
                error += e.Data + "\n";
            };

            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();

            var result = new CommandResult(output, error, proc.ExitCode);
            proc.Dispose();
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