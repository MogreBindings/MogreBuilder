using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Mogre.Builder.Tasks
{
    class OgreCmake : Task
    {
        public OgreCmake(OutputManager outputMgr) : base(outputMgr) { }

        public override string ID          { get { return "ogre:cmake"; } }
        public override string Name        { get { return "Running CMake on Ogre source tree"; } }
        public override string Description { get { return "Runs CMake on the Ogre source tree to generate the build environment"; } }

        public override void Run()
        {
            if (!Directory.Exists(@"Main\OgreSrc\build"))
            {
                Directory.CreateDirectory(@"Main\OgreSrc\build");
                var result = Cmd(@"cmake -DOGRE_CONFIG_ENABLE_PVRTC:BOOL=ON -OGRE_CONFIG_CONTAINERS_USE_CUSTOM_ALLOCATOR:BOOL=OFF -G ""Visual Studio 10"" ..\ogre", @"Main\OgreSrc\build");
                if (result.ExitCode != 0)
                    throw new UserException("Failed running CMake on Ogre source tree: " + result.Error);

                // Hack resulting solution file remove CMake's "Zero Check" project reference.
                var cmakeCache = File.ReadAllText(@"Main\OgreSrc\build\CMakeCache.txt");
                var match = Regex.Match(cmakeCache, @"ZERO_CHECK_GUID_CMAKE:\w+=(\S+)");
                if (!match.Success)
                {
                    outputMgr.Warn("Failed to find CMake zero check GUID, compilation might fail with unknown GUID error");
                    return;
                }
                var zeroCheckGuid = match.Groups[1].Value;

                ModifyFile(@"Main\OgreSrc\build\OGRE.sln", "^\\s*\\{" + zeroCheckGuid + "\\}.*$\\n", "", RegexOptions.Multiline);
            }
            else
            {
                outputMgr.Info("Ogre build directory already exists - skipping cmake.");
            }
        }
    }
}