using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class AssembleBinaryFiles : Task
    {
        public AssembleBinaryFiles(InputManager inputManager, OutputManager outputManager)
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "build:assemble"; } }
        public override string Name        { get { return "Assembling the generated binaries"; } }
        public override string Description { get { return "Assembles the final Ogre and Mogre binaries together in the target directory."; } }

        public override void Run()
        {            
            String targetDir = inputManager.BuildOutputDirectory;

            if (!Directory.Exists(targetDir))
                Directory.CreateDirectory(targetDir);

            String[] binPaths = new String[] 
            {
                String.Format(@"Main\lib\{0}", inputManager.BuildConfiguration),
                String.Format(@"Main\OgreSrc\build\bin\{0}", inputManager.BuildConfiguration),
                String.Format(@"Main\OgreSrc\build\lib\{0}", inputManager.BuildConfiguration),
                String.Format(@"MogreAddons\MOIS\MOIS\bin\{0}", inputManager.BuildConfiguration)
            };

            String[] patterns = new String[] { "*.dll", "*.lib", "*.pdb" };

            foreach (String binPath in binPaths)
            {
                // check if path exists
                if (Directory.Exists(binPath) == false)
                {
                    outputManager.Warning("Can't find binary directory:  " + binPath);
                    continue;
                }

                foreach (String pattern in patterns)
                {
                    foreach (String file in Directory.GetFiles(binPath, pattern))
                    {
                        try
                        {
                            String filePath = Path.Combine(targetDir, Path.GetFileName(file));

                            if (File.Exists(filePath))
                                File.Delete(filePath);

                            File.Move(file, filePath);
                        }
                        catch (IOException e)
                        {
                            outputManager.Warning(String.Format(
                                "Can't move binary file '{0}' to output directory. \n" + 
                                "    Source:     {1}  \n" + 
                                "    Exception:  {2}" , 
                                Path.GetFileName(file), Path.GetDirectoryName(file), e.Message));
                        }
                    }
                }
            } // foreach


            // warning if neither "Debug" nor "Release"
            if ((inputManager.BuildConfiguration.ToLower() != "debug") &&
                (inputManager.BuildConfiguration.ToLower() != "release"))
            {
                outputManager.Warning(String.Format(
                    "Note:  The build configuration is not 'Debug' and not 'Release'. (It's '{0}') \n " + 
                    "       Therefore it's possible, that some binary files are missed in the output directory (e.g. 'cg.dll'). \n" +
                    "       Consider to search and copy them manually.",
                    inputManager.BuildConfiguration));
            }

            outputManager.SuccessfulOgreBuild = true;

        } // Run()

    } // class AssembleBinaryFiles
} // namespace