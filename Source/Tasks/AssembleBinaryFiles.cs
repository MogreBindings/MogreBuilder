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
            String targetDir = String.Format(@"bin\{0}.NET4", inputManager.BuildConfiguration);

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            String[] binPaths = new String[] 
            {
                String.Format(@"Main\lib\{0}", inputManager.BuildConfiguration),
                String.Format(@"Main\OgreSrc\build\bin\{0}", inputManager.BuildConfiguration),
                String.Format(@"Main\OgreSrc\build\lib\{0}", inputManager.BuildConfiguration),
            };

            String[] patterns = new String[] { "*.dll", "*.lib", "*.pdb" };

            foreach (String binPath in binPaths)
            {
                foreach (String pattern in patterns)
                {
                    foreach (String entry in Directory.GetFiles(binPath, pattern))
                    {
                        String filePath = targetDir + "\\" + Path.GetFileName(entry);

                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        File.Move(entry, filePath);
                    }
                }
            } // foreach

        } // Run()

    } // class AssembleBinaryFiles
} // namespace