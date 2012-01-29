using System;
using System.IO;
using System.Collections.Generic;
using Mogre.Builder.Tasks;

namespace Mogre.Builder
{
    class Program
    {
        static void Main(string[] cmdLineArgs)
        {
            var outputMgr  = new ConsoleOutputManager();

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
                outputMgr.Error(e.Message);
            }
            finally
            {
                #if DEBUG
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                #endif
            }
        }

        private static CommandLineArgs ParseCommandLine(string[] cmdLineArgs)
        {
            var parsedArgs = new CommandLineArgs();

            if (cmdLineArgs.Length > 0)
                parsedArgs.TargetDir = cmdLineArgs[0];

            return parsedArgs;
        }

        private static List<Task> BuildTaskList(CommandLineArgs args, IOutputManager outputMgr)
        {
            var tasksToRun = new List<Task>(20);
            var msBuildMgr = new MsBuildManager(outputMgr);

            tasksToRun.Add(new CheckTargetDir(args.TargetDir, outputMgr));
            tasksToRun.Add(new CheckEnvironment(outputMgr));
            // Preparing Ogre code
            //CheckoutOgre();
            tasksToRun.Add(new PatchOgreCode(outputMgr));
            tasksToRun.Add(new OgreDependencies(outputMgr, msBuildMgr));
            tasksToRun.Add(new OgreCmake(outputMgr));
            // Auto-wrapping
            tasksToRun.Add(new AutoWrap(outputMgr, msBuildMgr));
            tasksToRun.Add(new AddClrClassesToOgre(outputMgr));
            // Building
            tasksToRun.Add(new BuildOgreWithoutMogreLinking(outputMgr, msBuildMgr));
            tasksToRun.Add(new BuildMogre(outputMgr, msBuildMgr));
            tasksToRun.Add(new BuildOgreWithMogreLinking(outputMgr, msBuildMgr));
            // Organizing the result
            tasksToRun.Add(new AssembleBinaryFiles(outputMgr));

            return tasksToRun;
        }
    }

    struct CommandLineArgs
    {
        public string TargetDir;
    }
}
