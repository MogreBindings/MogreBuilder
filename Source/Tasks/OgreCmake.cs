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

            if (result.ExitCode != 0)
                throw new UserException("Failed running CMake on Ogre source tree: " + result.Error);

            // Hack resulting solution file remove CMake's "Zero Check" project reference.
            String cmakeCache = File.ReadAllText(inputManager.CMakeCachePath);
            Match match = Regex.Match(cmakeCache, @"ZERO_CHECK_GUID_CMAKE:\w+=(\S+)");

            if (!match.Success)
            {
                outputManager.Warning("Failed to find CMake zero check GUID, compilation might fail with unknown GUID error");
                return;
            }
            String zeroCheckGuid = match.Groups[1].Value;

            ModifyFile(inputManager.OgreSolutionFile, "^\\s*\\{" + zeroCheckGuid + "\\}.*$\\n", "", RegexOptions.Multiline);
        }
    }
}