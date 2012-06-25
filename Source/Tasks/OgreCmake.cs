using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Mogre.Builder.Tasks
{
    class OgreCmake : Task
    {
        public OgreCmake(InputManager inputManager, IOutputManager outputManager) 
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "ogre:cmake"; } }
        public override string Name        { get { return "Running CMake on Ogre source tree"; } }
        public override string Description { get { return "Runs CMake on the Ogre source tree to generate the build environment"; } }

        public override void Run()
        {
            // we always want to do this otherwise switching between configurations fails (e.g. Debug then Release).
            Directory.CreateDirectory(inputManager.OgreBuildDirectory);

            String cmakeArguments =
                @"-DOGRE_CONFIG_ENABLE_PVRTC:BOOL=ON -OGRE_CONFIG_CONTAINERS_USE_CUSTOM_ALLOCATOR:BOOL=OFF " +
                @"-DCMAKE_DISABLE_FIND_PACKAGE_PkgConfig:BOOL=TRUE -DCMAKE_DISABLE_FIND_PACKAGE_Doxygen:BOOL=TRUE " +
                @"-G ""Visual Studio 10"" " +
                @"..\ogre";

            // run CMake
            CommandResult result = RunCommand(inputManager.CMakeExecutable, cmakeArguments, inputManager.OgreBuildDirectory);

            if (result.ExitCode != 0)
            {
                // check for problem "directory changed"
                if (Regex.IsMatch(result.Error, @"The current CMakeCache\.txt directory .* is different than the directory"))
                    outputManager.Warning("Suggestion for this CMake error:  Delete the target directory and try again.");

                throw new UserException("\nFailed running CMake on Ogre source tree: \n" + result.Error);
            }


            //--- run CMake again to include Ogre dependencies ---

            outputManager.Action("Running CMake again to include fresh built Ogre depencies");

            // clear the catched feature list of first CMake run
            outputManager.FeatureSummary = "";

            // run CMake
            result = RunCommand(inputManager.CMakeExecutable, cmakeArguments, inputManager.OgreBuildDirectory);

            if (result.ExitCode != 0)
                throw new UserException("\nFailed running CMake on Ogre source tree: \n" + result.Error);
            
        } // Run()
    }
}