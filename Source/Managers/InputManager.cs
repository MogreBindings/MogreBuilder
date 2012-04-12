using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace Mogre.Builder
{
    public partial class InputManager
    {
        public InputManager(string targetDirectory)
        {
            TargetDirectory = targetDirectory;
            LoadDefaults();
        }



        public InputManager(string targetDirectory, CommandLineArgs parsedArgs)
            : this(targetDirectory)
        {
            // copy possibly modified PATH variable
            this.PathEnvironmentVariable = parsedArgs.PathEnvironmentVariable;

            // copy some option flags
            Option_OnlyAddons      = parsedArgs.OnlyAddons;
            Option_MogreNewt       = parsedArgs.MogreNewt;
            Option_DevelopmentFlag = parsedArgs.DevelopmentFlag;

            // load config file (if defined as argument)
            if(parsedArgs.ConfigFile != null)
                LoadConfig(parsedArgs.ConfigFile);
        }
        




        // environment variable PATH
        public String PathEnvironmentVariable  // don't add this to config file
        {
            get { return pathEnvironmentVariable; }
            set { pathEnvironmentVariable = value; }
        }
        private String pathEnvironmentVariable = Environment.GetEnvironmentVariable("path");  // system default

        public Boolean Option_OnlyAddons { get; set; }
        public Boolean Option_MogreNewt { get; set; }
        public Boolean Option_DevelopmentFlag { get; set; }

        // currently unsupported add-ons of the official add-ons repository
        public Boolean Option_Hikari { get; set; }
        public Boolean Option_Makuri { get; set; }
        public Boolean Option_MogreDesignSupport { get; set; }
        public Boolean Option_MogreFreeSL { get; set; }



        // build
        public string TargetDirectory { get; private set; }
        public string BuildConfiguration { get; private set; }
        public string BuildOutputDirectory { get; private set; }  // don't add this to config file  (will be generated automatically)

        // clr
        public string ClrDirectory { get; private set; }
        public string ClrConfigHeaderFile { get; private set; }
        public string ClrObjectsBuildFile { get; private set; }
        public string ClrObjectsAutoFile { get; private set; }
        public string ClrObjectsBuildDirectory { get; private set; }

        // mogre
        public string MogreAssemblyInfoFile { get; private set; }
        public string MogreSolutionFile { get; private set; }
        public string MogreRepository { get; private set; }
        public string MogreBranch { get; private set; }

        // ogre
        public string OgreRootDirectory { get; private set; }
        public string OgreBuildDirectory { get; private set; }
        public string OgreMainDirectory { get; private set; }
        public string OgreIncludeDirectory { get; private set; }
        public string OgreSourceDirectory { get; private set; }
        public string OgrePrequisitesFile { get; private set; }
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
        public string ClrPatchFile { get; private set; }
        public string CygonPatchFile { get; private set; }

        // cpp2java
        public string Cpp2JavaDirectory { get; private set; }
        public string Cpp2JavaMetaDataFile { get; private set; }

        // autowrap
        public string AutoWrapExecutable { get; private set; }
        public string AutoWrapSolutionFile { get; private set; }
        public string AutoWrappedCodeDirectory { get; private set; }
        public string AutoWrapWorkingDirectory { get; private set; }


        // Add-ons
        public String MogreAddonsRepository { get; private set; }
        public String MogreAddonsDirectory { get; private set; }


        //--- MogreNewt ---
        
        public String NewtonMainDirectory                         { get; private set; }
        
        public String NewtonInputDirectory_Mogre_binary_debug     { get; private set; }
        public String NewtonInputDirectory_Mogre_binary_release   { get; private set; }
        public String NewtonInputDirectory_Ogre_headers           { get; private set; }
        public String NewtonInputDirectory_Ogre_headers_Threading { get; private set; }
        public String NewtonInputDirectory_Ogre_headers_Win32     { get; private set; }
        public String NewtonInputDirectory_OgreMain_binaries      { get; private set; }
        public String NewtonInputDirectory_OgreMain_headers       { get; private set; }
        public String NewtonInputDirectory_NewtonLibrary          { get; private set; }

        public String NewtonDepencySourceDir_Mogre_binary           { get; private set; }  // don't add this to config file  (will be generated automatically)
        public String NewtonDepencySourceDir_Ogre_headers           { get; private set; }  // don't add this to config file
        public String NewtonDepencySourceDir_Ogre_headers_Threading { get; private set; }  // don't add this to config file
        public String NewtonDepencySourceDir_Ogre_headers_Win32     { get; private set; }  // don't add this to config file
        public String NewtonDepencySourceDir_OgreMain_binaries      { get; private set; }  // don't add this to config file
        public String NewtonDepencySourceDir_OgreMain_headers       { get; private set; }  // don't add this to config file
        public String NewtonDepencySourceDir_NewtonLibrary          { get; private set; } 




        /// <summary>
        /// Generate some path variables, which are composed by other path variables
        /// </summary>
        public void GeneratePathVariables()
        {
            // ----- Common -----

            BuildOutputDirectory = String.Format(@"bin\{0}.NET4", BuildConfiguration);


            // ----- Add-ons -----

            if (Path.IsPathRooted(MogreAddonsDirectory) == false)
            {
                MogreAddonsDirectory = Path.Combine(TargetDirectory, MogreAddonsDirectory);
            }



            // ----- MogreNewt add-on -----

            // NewtonMainDirectory
            if (Path.IsPathRooted(NewtonMainDirectory))
            {
                // absolute directory   (Users can modify this config entry when they want to use a custom MogreNewt directory)
                // --> do nothing
            }
            else
            {
                // relative directory
                // --> adjust to path of cloned HG directory
                NewtonMainDirectory = Path.Combine(MogreAddonsDirectory, NewtonMainDirectory);
            }

            // get directories where to find the needed depency files
            NewtonDepencySourceDir_Mogre_binary           = BuildOutputDirectory;
            NewtonDepencySourceDir_Ogre_headers           = OgreIncludeDirectory;
            NewtonDepencySourceDir_Ogre_headers_Threading = Path.Combine(OgreIncludeDirectory, "Threading");
            NewtonDepencySourceDir_Ogre_headers_Win32     = Path.Combine(OgreIncludeDirectory, "WIN32");
            NewtonDepencySourceDir_OgreMain_binaries      = BuildOutputDirectory;
            NewtonDepencySourceDir_OgreMain_headers       = ClrObjectsBuildDirectory;
            
            NewtonDepencySourceDir_NewtonLibrary = "";  // TODO: Maybe add a task to download them from web (e.g. from a zip archive)


            //--- make paths absolute ---

            // relative to TargetDirectory
            NewtonDepencySourceDir_Mogre_binary           = Path.Combine(TargetDirectory, NewtonDepencySourceDir_Mogre_binary);
            NewtonDepencySourceDir_Ogre_headers           = Path.Combine(TargetDirectory, NewtonDepencySourceDir_Ogre_headers);
            NewtonDepencySourceDir_Ogre_headers_Threading = Path.Combine(TargetDirectory, NewtonDepencySourceDir_Ogre_headers_Threading);
            NewtonDepencySourceDir_Ogre_headers_Win32     = Path.Combine(TargetDirectory, NewtonDepencySourceDir_Ogre_headers_Win32);
            NewtonDepencySourceDir_OgreMain_binaries      = Path.Combine(TargetDirectory, NewtonDepencySourceDir_OgreMain_binaries);
            NewtonDepencySourceDir_OgreMain_headers       = Path.Combine(TargetDirectory, NewtonDepencySourceDir_OgreMain_headers);

            // relative to NewtonMainDirectory  (can be outside of the TargetDirectory)
            NewtonInputDirectory_Mogre_binary_debug       = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_Mogre_binary_debug);
            NewtonInputDirectory_Mogre_binary_release     = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_Mogre_binary_release);
            NewtonInputDirectory_NewtonLibrary            = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_NewtonLibrary);
            NewtonInputDirectory_Ogre_headers             = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_Ogre_headers);
            NewtonInputDirectory_Ogre_headers_Threading   = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_Ogre_headers_Threading);
            NewtonInputDirectory_Ogre_headers_Win32       = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_Ogre_headers_Win32);
            NewtonInputDirectory_OgreMain_binaries        = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_OgreMain_binaries);
            NewtonInputDirectory_OgreMain_headers         = Path.Combine(NewtonMainDirectory, NewtonInputDirectory_OgreMain_headers);


        } //  GeneratePathVariables()




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
            // check if file exists
            if ((configFile == null) || (File.Exists(configFile) == false))
                throw new Exception(String.Format("Config file not found:  {0}", configFile));

            try
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                StreamReader reader = new StreamReader(configFile);
                string line = reader.ReadLine();

                while (line != null)
                {
                    line = line.Trim();

                    // skip pure comment lines and blank lines
                    if (!line.StartsWith(@"//") && !string.IsNullOrEmpty(line))
                    {
                        // remove comments after line of code
                        String commentPattern = @";\s*//.*";
                        if (Regex.IsMatch(line, commentPattern))
                            line = Regex.Replace(line, commentPattern, ";");

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
                throw new Exception(String.Format(
                    "Error loading config file '{0}' \n" +
                    "    Reason: {1}", configFile, ex.Message));
            }
        } // LoadKeyValueDictionary()

    } // class InputManager
}
