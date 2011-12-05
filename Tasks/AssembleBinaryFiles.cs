using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class AssembleBinaryFiles : Task
    {
        public AssembleBinaryFiles(OutputManager outputMgr) : base(outputMgr) { }

        public override string ID          { get { return "build:assemble"; } }
        public override string Name        { get { return "Assembling the generated binaries"; } }
        public override string Description { get { return "Assembles the final Ogre and Mogre binaries together in the target directory."; } }

        public override void Run()
        {
            var targetDir = @"bin\Debug.NET4";

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var binPaths = new string[] {
                @"Main\lib\Debug",
                @"Main\OgreSrc\build\bin\debug",
                @"Main\OgreSrc\build\lib\Debug",
            };
            var patterns = new string[] { "*.dll", "*.lib", "*.pdb" };

            foreach (var binPath in binPaths)
                foreach (var pattern in patterns)
                    foreach (var entry in Directory.GetFiles(binPath, pattern))
                        File.Move(entry, targetDir + "\\" + Path.GetFileName(entry));
        }
    }
}