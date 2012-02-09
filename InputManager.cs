using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Mogre.Builder
{
    public class InputManager
    {
        public InputManager(string targetDirectory)
        {
            TargetDirectory = targetDirectory;
            LoadDefaults();
        }

        public InputManager(string targetDirectory, string configFile)
            : this(targetDirectory)
        {
            if(configFile != null)
                LoadConfig(configFile);
        }
        
        // build
        public string TargetDirectory { get; private set; }
        public string BuildConfiguration { get; private set; }

        // clr
        public string ClrDirectory { get; private set; }
        public string ClrConfigHeaderFile { get; private set; }
        public string ClrObjectsBuildFile { get; private set; }
        public string ClrObjectsAutoFile { get; private set; }

        // mogre
        public string MogreSolutionFile { get; private set; }
        public string MogreRepository { get; private set; }
        public string MogreBranch { get; private set; }

        // ogre
        public string OgreRootDirectory { get; private set; }
        public string OgreBuildDirectory { get; private set; }
        public string OgreMainDirectory { get; private set; }
        public string OgreIncludeDirectory { get; private set; }
        public string OgreSourceDirectory { get; private set; }
        public string OgreProjectFile { get; private set; }
        public string OgreSolutionFile { get; private set; }
        public string OgreRepository { get; private set; }
        public string OgreBranch { get; private set; }

        // cmake
        public string CMakeExecutable { get; private set; }
        public string CMakeCachePath { get; private set; }

        // dependencies
        public string DependenciesURL { get; private set; }
        public string DependenciesZip { get; private set; }
        public string DependenciesDirectory { get; private set; }
        public string DependenciesSolutionFile { get; private set; }

        // patch
        public string PatchExecutable { get; private set; }
        public string PatchFile { get; private set; }

        // cpp2java
        public string Cpp2JavaDirectory { get; private set; }
        public string Cpp2JavaMetaDataFile { get; private set; }

        // autowrap
        public string AutoWrapExecutable { get; private set; }
        public string AutoWrapSolutionFile { get; private set; }
        public string AutoWrappedCodeDirectory { get; private set; }
        public string AutoWrapWorkingDirectory { get; private set; }

        private void LoadDefaults()
        {
            // build
            BuildConfiguration = "Release";

            // clr
            ClrDirectory = @"Main\Ogre\";
            ClrConfigHeaderFile = @"Main\OgreSrc\ogre\OgreMain\include\CLRConfig.h";
            ClrObjectsBuildFile = @"Main\OgreSrc\build\include\CLRObjects.inc";
            ClrObjectsAutoFile = @"Main\include\auto\CLRObjects.inc";

            // mogre
            MogreSolutionFile = @"Main\Mogre_vs2010.sln";
            MogreRepository = @"https://bitbucket.org/mogre/mogre/";
            MogreBranch = @"default";

            // ogre
            OgreRootDirectory = @"Main\OgreSrc\ogre\";
            OgreBuildDirectory = @"Main\OgreSrc\build\";
            OgreMainDirectory = @"Main\OgreSrc\ogre\OgreMain\";
            OgreIncludeDirectory = @"Main\OgreSrc\ogre\OgreMain\include\";
            OgreSourceDirectory = @"Main\OgreSrc\ogre\OgreMain\src\";
            OgreProjectFile = @"Main\OgreSrc\build\OgreMain\OgreMain.vcxproj";
            OgreSolutionFile = @"Main\OgreSrc\build\OGRE.sln";
            OgreRepository = @"https://bitbucket.org/sinbad/ogre/";
            OgreBranch = @"v1-7";

            // cmake
            CMakeExecutable = @"C:\Program Files (x86)\CMake 2.8\bin\cmake.exe"; ;
            CMakeCachePath = @"Main\OgreSrc\build\CMakeCache.txt";

            // dependencies
            DependenciesURL = "http://surfnet.dl.sourceforge.net/project/ogre/ogre-dependencies-vc%2B%2B/1.7/OgreDependencies_MSVC_20100501.zip";
            DependenciesZip = @"Main\OgreSrc\ogre\Dependencies.zip";
            DependenciesDirectory = @"Main\OgreSrc\ogre\Dependencies\";
            DependenciesSolutionFile = @"Main\OgreSrc\ogre\Dependencies\src\OgreDependencies.VS2010.sln";

            // patch
            PatchExecutable = "pat-ch.exe";
            PatchFile = @"Main\Ogre Patches\58266f25ccd2.patch";

            // cpp2java
            Cpp2JavaDirectory = @"Codegen\cpp2java";
            Cpp2JavaMetaDataFile = @"Codegen\cpp2java\build\all.xml";

            // autowrap
            AutoWrapExecutable = @"Codegen\AutoWrap\bin\Debug\AutoWrap.exe";
            AutoWrapSolutionFile = @"Codegen\AutoWrap\AutoWrap_vs2010.sln";
            AutoWrappedCodeDirectory = @"Main\src\auto\";
            AutoWrapWorkingDirectory = @"Codegen\AutoWrap\bin\Debug\";
        }

        public void LoadConfig(string configFile)
        {
            Dictionary<string, string> dictionary = LoadKeyValueDictionary(configFile);

            foreach (PropertyInfo propertyInfo in GetType().GetProperties())
            {
                if (dictionary.ContainsKey(propertyInfo.Name))
                {
                    propertyInfo.SetValue(this, dictionary[propertyInfo.Name], null);
                }
            }
        }

        private Dictionary<string, string> LoadKeyValueDictionary(string configFile)
        {
            try
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                StreamReader reader = new StreamReader(configFile);
                string line = reader.ReadLine();

                while (line != null)
                {
                    line = line.Trim();

                    // skip comments and blanks
                    if (!line.StartsWith(@"//") && !string.IsNullOrEmpty(line))
                    {
                        string[] bits = line.Split('=');

                        if (bits.Length != 2)
                            throw new Exception(string.Format("Error reading config line\r\n{0}", line));

                        string key = bits[0].Trim();
                        string value = bits[1].Trim().Trim('"', '@', ';');

                        dictionary.Add(key, value);
                    }

                    line = reader.ReadLine();
                }

                reader.Close();
                return dictionary;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error loading config file {0}", configFile), ex);
            }
        }
    }
}
