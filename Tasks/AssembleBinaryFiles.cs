using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class AssembleBinaryFiles : Task
    {
        public AssembleBinaryFiles(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "build:assemble"; } }
        public override string Name        { get { return "Assembling the generated binaries"; } }
        public override string Description { get { return "Assembles the final Ogre and Mogre binaries together in the target directory."; } }

        public override void Run()
        {            
            var targetDir = string.Format(@"bin\{0}.NET4", inputManager.BuildConfiguration);

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            var binPaths = new string[] 
            {
                string.Format(@"Main\lib\{0}", inputManager.BuildConfiguration),
                string.Format(@"Main\OgreSrc\build\bin\{0}", inputManager.BuildConfiguration),
                string.Format(@"Main\OgreSrc\build\lib\{0}", inputManager.BuildConfiguration),
            };

            var patterns = new string[] { "*.dll", "*.lib", "*.pdb" };

            foreach (var binPath in binPaths)
            {
                foreach (var pattern in patterns)
                {
                    foreach (var entry in Directory.GetFiles(binPath, pattern))
                    {
                        string filePath = targetDir + "\\" + Path.GetFileName(entry);

                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        File.Move(entry, filePath);
                    }
                }
            }
        }
    }
}