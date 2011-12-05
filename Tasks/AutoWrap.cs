using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class AutoWrap : MsBuildTask
    {
        public AutoWrap(OutputManager outputMgr, MsBuildManager msBuildMgr) : base(outputMgr, msBuildMgr) { }

        public override string ID          { get { return "mogre:autowrap"; } }
        public override string Name        { get { return "Autowrapping Ogre classes"; } }
        public override string Description { get { return "Scans the Ogre source tree using cpp2java and then builds and runs the autowrapper to automatically generate Mogre wrapping classes for certain Ogre classes."; } }

        public override void Run()
        {
            if (!File.Exists(@"Codegen\cpp2java\build\all.xml"))
            {
                outputMgr.Info("Running cpp2java to scan Ogre source tree and build meta-data");
                Cmd("build.bat", @"Codegen\cpp2java");
            }
            else
            {
                outputMgr.Info("cpp2java meta-data appears to aleady been generated, skipping cpp2java");
            }

            if (!File.Exists(@"Codegen\AutoWrap\bin\Debug\AutoWrap.exe"))
            {
                outputMgr.Info("Building Mogre Autowrapper");
                msBuildMgr.Build(@"Codegen\AutoWrap\AutoWrap_vs2010.sln", "Debug", "Any CPU", "Build");
            }
            else
            {
                outputMgr.Info("Mogre Autowrapper appears to already exist, skipping");
            }

            if (!Directory.Exists(@"Main\src\auto"))
            {
                outputMgr.Info("Running Mogre Autowrapper");
                Cmd(@"Codegen\AutoWrap\bin\Debug\AutoWrap.exe produce", @"Codegen\AutoWrap\bin\Debug");
            }
            else
            {
                outputMgr.Info("Mogre auto-wrapped code appears to have been generated, skipping autowrap");
            }
        }
    }
}