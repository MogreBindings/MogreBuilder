using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Build.Evaluation;

namespace Mogre.Builder.Tasks
{
    class AddClrClassesToOgre : Task
    {
        public AddClrClassesToOgre(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager)
        { 
        }

        public override string ID          { get { return "mogre:copy-clr"; } }
        public override string Name        { get { return "Copying additional CLR code to Ogre Source tree"; } }
        public override string Description { get { return "Copies certain additional CLR source code to the Ogre source tree and adds them to the Ogre build."; } }

        public override void Run()
        {
            if (!File.Exists(inputManager.ClrConfigHeaderFile) || !File.Exists(inputManager.ClrObjectsBuildFile))
            {
                outputManager.Info("Copying Mogre files to Ogre source tree");

                foreach (String file in Directory.GetFiles(inputManager.ClrDirectory, "*.h"))
                    File.Copy(file, inputManager.OgreIncludeDirectory + Path.GetFileName(file), true);

                foreach (String file in Directory.GetFiles(inputManager.ClrDirectory, "*.cpp"))
                    File.Copy(file, inputManager.OgreSourceDirectory + Path.GetFileName(file), true);

                File.Copy(inputManager.ClrObjectsAutoFile, inputManager.ClrObjectsBuildFile, true);
            }
            else
            {
                outputManager.Info("Mogre's additional Ogre source files appear to already have been copied to Ogre source tree, skipping");
            }

            Dictionary<String, Boolean> sourceFiles = new Dictionary<String, Boolean>{
                {"CLRHandle.cpp", false},
                {"CLRObject.cpp", false}
            };

            Dictionary<String, Boolean> includeFiles = new Dictionary<String, Boolean>{
                {"CLRConfig.h", false},
                {"CLRHandle.h", false},
                {"CLRObject.h", false}
            };

            Project p = new Project(inputManager.OgreProjectFile);

            foreach (ProjectItem item in p.GetItems("ClCompile"))
            {
                String file = Path.GetFileName(item.EvaluatedInclude);

                if (sourceFiles.ContainsKey(file))
                    sourceFiles[file] = true;
            }

            foreach (ProjectItem item in p.GetItems("ClInclude"))
            {
                String file = Path.GetFileName(item.EvaluatedInclude);

                if (includeFiles.ContainsKey(file))
                    includeFiles[file] = true;
            }

            Boolean projectModified = false;
            foreach (KeyValuePair<String, Boolean> pair in sourceFiles)
            {
                String file = pair.Key;
                Boolean included = pair.Value;
                if (!included)
                {
                    if (!projectModified)
                        outputManager.Info("Adding Mogre's additional Ogre source files");

                    p.AddItem("ClCompile", @"..\..\ogre\OgreMain\src\" + file);
                    projectModified = true;
                }
            }

            foreach (KeyValuePair<String, Boolean> pair in includeFiles)
            {
                String file = pair.Key;
                Boolean included = pair.Value;
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