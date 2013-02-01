using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class AutoWrap : MsBuildTask
    {
        public AutoWrap(InputManager inputManager, OutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr) 
        { 
        }

        public override string ID          { get { return "mogre:autowrap"; } }
        public override string Name        { get { return "Autowrapping Ogre classes"; } }
        public override string Description { get { return "Scans the Ogre source tree using cpp2java and then builds and runs the autowrapper to automatically generate Mogre wrapping classes for certain Ogre classes."; } }

        public override void Run()
        {
            if (!File.Exists(inputManager.Cpp2JavaMetaDataFile))
            {
                outputManager.Info("Running cpp2java to scan Ogre source tree and build meta-data");
                RunCommand("cmd", "/c build.bat", inputManager.Cpp2JavaDirectory);
            }
            else
            {
                outputManager.Info("cpp2java meta-data appears to have already been generated, skipping cpp2java");
            }

            if (!File.Exists(inputManager.AutoWrapExecutable))
            {
                outputManager.Info("Building Mogre Autowrapper");
                msBuildManager.Build(inputManager.AutoWrapSolutionFile, "Debug", "Any CPU", "Build");
            }
            else
            {
                outputManager.Info("Mogre Autowrapper appears to already exist, skipping");
            }

            if (!Directory.Exists(inputManager.AutoWrappedCodeDirectory))
            {
                outputManager.Info("Running Mogre Autowrapper");
                RunCommand(inputManager.AutoWrapExecutable, "produce", inputManager.AutoWrapWorkingDirectory);
            }
            else
            {
                outputManager.Info("Mogre auto-wrapped code appears to have been generated, skipping autowrap");
            }
        }
    }
}