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

            // set build configuration for CMake
            String cmakeArguments =
                "-DCMAKE_BUILD_TYPE:STRING=" + inputManager.BuildConfiguration + " " +
                "-DCMAKE_CONFIGURATION_TYPES=" + inputManager.BuildConfiguration + " ";

            // set needed options for CMake
            cmakeArguments +=
                "-DOGRE_CONFIG_ENABLE_PVRTC:BOOL=ON " + // enable PVRTC codec (texture compression)
                "-OGRE_CONFIG_CONTAINERS_USE_CUSTOM_ALLOCATOR:BOOL=OFF "; // use default allocator

            // disable unnecessary features
            cmakeArguments +=
                "-DCMAKE_DISABLE_FIND_PACKAGE_PkgConfig:BOOL=ON " + // don't use PkgConfig (not available on Windows)
                "-DCMAKE_DISABLE_FIND_PACKAGE_Doxygen:BOOL=ON " +   // don't search for Doxygen
                "-DOGRE_BUILD_TOOLS:BOOL=OFF " +                    // don't build tools
                "-DOGRE_BUILD_SAMPLES:BOOL=OFF " +                  // don't build samples
                "-DOGREDEPS_BUILD_OIS:BOOL=OFF ";                   // disable OIS (will be built separately for MOIS)

            // disable all installation targets
            cmakeArguments +=
                "-DOGREDEPS_INSTALL_DEV:BOOL=OFF " +
                "-DOGRE_INSTALL_DEPENDENCIES:BOOL=OFF " +
                "-DOGRE_INSTALL_DOCS:BOOL=OFF " +
                "-DOGRE_INSTALL_PDB:BOOL=OFF " +
                "-DOGRE_INSTALL_SAMPLES:BOOL=OFF " +
                "-DOGRE_INSTALL_SAMPLES_SOURCE:BOOL=OFF ";

            // add generator and target path
            cmakeArguments += "-G \"" + generator + "\" " + @"..\ogre";
            
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