using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace Mogre.Builder.Tasks
{
    class AddClrClassesToOgre : Task
    {
        public AddClrClassesToOgre(IOutputManager outputMgr) : base(outputMgr) { }

        public override string ID          { get { return "mogre:copy-clr"; } }
        public override string Name        { get { return "Copying additional CLR code to Ogre Source tree"; } }
        public override string Description { get { return "Copies certain additional CLR source code to the Ogre source tree and adds them to the Ogre build."; } }

        public override void Run()
        {
            if (!File.Exists(@"Main\OgreSrc\ogre\OgreMain\include\CLRConfig.h") ||
                !File.Exists(@"Main\OgreSrc\build\include\CLRObjects.inc"))
            {
                outputManager.Info("Copying Mogre files to Ogre source tree");
                foreach (var file in Directory.GetFiles(@"Main\Ogre", "*.h"))
                    File.Copy(file, @"Main\OgreSrc\ogre\OgreMain\include\" + Path.GetFileName(file), true);
                foreach (var file in Directory.GetFiles(@"Main\Ogre", "*.cpp"))
                    File.Copy(file, @"Main\OgreSrc\ogre\OgreMain\src\" + Path.GetFileName(file), true);
                File.Copy(@"Main\include\auto\CLRObjects.inc", @"Main\OgreSrc\build\include\CLRObjects.inc", true);
            }
            else
            {
                outputManager.Info("Mogre's additional Ogre source files appear to already have been copied to Ogre source tree, skipping");
            }

            var sourceFiles = new Dictionary<string, bool>{
                {"CLRHandle.cpp", false},
                {"CLRObject.cpp", false}
            };

            var includeFiles = new Dictionary<string, bool>{
                {"CLRConfig.h", false},
                {"CLRHandle.h", false},
                {"CLRObject.h", false}
            };

            var p = new Project(@"Main\OgreSrc\build\OgreMain\OgreMain.vcxproj");
            foreach (var item in p.GetItems("ClCompile"))
            {
                var file = Path.GetFileName(item.EvaluatedInclude);
                if (sourceFiles.ContainsKey(file))
                    sourceFiles[file] = true;
            }
            foreach (var item in p.GetItems("ClInclude"))
            {
                var file = Path.GetFileName(item.EvaluatedInclude);
                if (includeFiles.ContainsKey(file))
                    includeFiles[file] = true;
            }

            var projectModified = false;
            foreach (var pair in sourceFiles)
            {
                var file = pair.Key;
                var included = pair.Value;
                if (!included)
                {
                    if (!projectModified)
                        outputManager.Info("Adding Mogre's additional Ogre source files");
                    p.AddItem("ClCompile", @"..\..\ogre\OgreMain\src\" + file);
                    projectModified = true;
                }
            }

            foreach (var pair in includeFiles)
            {
                var file = pair.Key;
                var included = pair.Value;
                if (!included)
                {
                    if (!projectModified)
                        outputManager.Info("Adding Mogre's additional Ogre source files");
                    p.AddItem("ClInclude", @"..\..\ogre\OgreMain\include\" + file);
                    projectModified = true;
                }
            }

            if (projectModified)
                p.Save();
        }
    }
}