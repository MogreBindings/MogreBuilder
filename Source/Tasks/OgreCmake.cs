using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Mogre.Builder.Tasks
{
    class OgreCmake : Task
    {
        public OgreCmake(InputManager inputManager, OutputManager outputManager) 
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "ogre:cmake"; } }
        public override string Name        { get { return "Running CMake on Ogre source tree (errors from missing dependencies expected!)"; } }
        public override string Description { get { return "Runs CMake on the Ogre source tree to generate the build environment"; } }

        public override void Run()
        {
            // we always want to do this otherwise switching between configurations fails (e.g. Debug then Release).
            Directory.CreateDirectory(inputManager.OgreBuildDirectory);

            string generator = inputManager.Option_Vs2012 ? "Visual Studio 11" : "Visual Studio 10";

            if (inputManager.Option_x64)
                generator += " Win64";

            String cmakeArguments =
                @"-DOGRE_CONFIG_ENABLE_PVRTC:BOOL=ON -OGRE_CONFIG_CONTAINERS_USE_CUSTOM_ALLOCATOR:BOOL=OFF " +
                @"-DCMAKE_DISABLE_FIND_PACKAGE_PkgConfig:BOOL=TRUE -DCMAKE_DISABLE_FIND_PACKAGE_Doxygen:BOOL=TRUE " +
                @"-DOGRE_INSTALL_DEPENDENCIES:BOOL=OFF " +
                 "-G \"" + generator + "\" " +
                @"..\ogre";
            
            // run CMake (with disabled output processing to not record errors) 
            outputManager.IsErrorRecordingEnabled = false;
            RunCommand(inputManager.CMakeExecutable, cmakeArguments, inputManager.OgreBuildDirectory);
            outputManager.IsErrorRecordingEnabled = true;

            outputManager.Action("Running CMake again to include fresh built Ogre depencies");

            // run CMake
            CommandResult result = RunCommand(inputManager.CMakeExecutable, cmakeArguments, inputManager.OgreBuildDirectory);

            if (result.ExitCode != 0)
                throw new UserException("\nFailed running CMake on Ogre source tree: \n" + result.Error);
            
        } // Run()
    }
}