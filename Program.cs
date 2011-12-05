using System;
using System.IO;
using System.Collections.Generic;

namespace Mogre.Builder
{
    class Program
    {
        static void Main(string[] cmdLineArgs)
        {
            var outputMgr  = new OutputManager();

            try
            {
                var parsedArgs = ParseCommandLine(cmdLineArgs);
                var tasksToRun = BuildTaskList(parsedArgs, outputMgr);

                foreach (var task in tasksToRun)
                {
                    outputMgr.Action(task.Name);
                    task.Run();
                }
            }
            catch (UserException e)
            {
                outputMgr.DisplayMessage(e.Message, ConsoleColor.Red);
            }
        }

        private static CommandLineArgs ParseCommandLine(string[] cmdLineArgs)
        {
            var parsedArgs = new CommandLineArgs();

            if (cmdLineArgs.Length > 0)
                parsedArgs.TargetDir = cmdLineArgs[0];

            return parsedArgs;
        }

        private static List<Task> BuildTaskList(CommandLineArgs args, OutputManager outputMgr)
        {
            var tasksToRun = new List<Task>(20);
            var msBuildMgr = new MsBuildManager(outputMgr);

            tasksToRun.Add(new Tasks.CheckTargetDir(args.TargetDir, outputMgr));
            tasksToRun.Add(new Tasks.CheckEnvironment(outputMgr));
            // Preparing Ogre code
            //CheckoutOgre();
            tasksToRun.Add(new Tasks.PatchOgreCode(outputMgr));
            tasksToRun.Add(new Tasks.OgreDependencies(outputMgr, msBuildMgr));
            tasksToRun.Add(new Tasks.OgreCmake(outputMgr));
            // Auto-wrapping
            tasksToRun.Add(new Tasks.AutoWrap(outputMgr, msBuildMgr));
            tasksToRun.Add(new Tasks.AddClrClassesToOgre(outputMgr));
            // Building
            tasksToRun.Add(new Tasks.BuildOgreWithoutMogreLinking(outputMgr, msBuildMgr));
            tasksToRun.Add(new Tasks.BuildMogre(outputMgr, msBuildMgr));
            tasksToRun.Add(new Tasks.BuildOgreWithMogreLinking(outputMgr, msBuildMgr));
            // Organizing the result
            tasksToRun.Add(new Tasks.AssembleBinaryFiles(outputMgr));

            return tasksToRun;
        }
    }

    struct CommandLineArgs
    {
        public string TargetDir;
    }
}
