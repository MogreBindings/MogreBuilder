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
            CommandResult result = RunCommand(inputManager.CMakeExecutable, 
                @"-DOGRE_CONFIG_ENABLE_PVRTC:BOOL=ON -OGRE_CONFIG_CONTAINERS_USE_CUSTOM_ALLOCATOR:BOOL=OFF -G ""Visual Studio 10"" ..\ogre",
                inputManager.OgreBuildDirectory);

            // run CMake again to include Ogre dependencies
            result = RunCommand(inputManager.CMakeExecutable,
                @"-DOGRE_CONFIG_ENABLE_PVRTC:BOOL=ON -OGRE_CONFIG_CONTAINERS_USE_CUSTOM_ALLOCATOR:BOOL=OFF -G ""Visual Studio 10"" ..\ogre",
                inputManager.OgreBuildDirectory);

            if (result.ExitCode != 0)
            {
                // check for problem "directory changed"
                if (Regex.IsMatch(result.Error, @"The current CMakeCache\.txt directory .* is different than the directory"))
                    outputManager.Warning("Suggestion for this CMake error:  Delete the target directory and try again.");

                throw new UserException("\nFailed running CMake on Ogre source tree: \n" + result.Error);
            }
        }
    }
}