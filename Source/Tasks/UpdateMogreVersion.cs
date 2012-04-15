using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class UpdateMogreVersion : Task
    {
        public UpdateMogreVersion(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "mogre:updateVersion"; } }
        public override string Name        { get { return "Updating Mogre Version"; } }
        public override string Description { get { return "Detects the version of Ogre and updates Mogre to match."; } }
                
        public override void Run()
        {
            StreamReader reader = new StreamReader(inputManager.OgrePrequisitesFile);
            string line = reader.ReadLine();
            string versionName = "", major = "", minor = "", suffix = "", patch = "";

            while (line != null)
            {
                line = line.Trim();

                if(line.StartsWith("#define OGRE_VERSION_MAJOR"))
                    major = ExtractDefineValue(line);

                if(line.StartsWith("#define OGRE_VERSION_MINOR"))
                    minor = ExtractDefineValue(line);

                if(line.StartsWith("#define OGRE_VERSION_PATCH"))
                    patch = ExtractDefineValue(line);

                if(line.StartsWith("#define OGRE_VERSION_SUFFIX"))
                    suffix = ExtractDefineValue(line);

                if(line.StartsWith("#define OGRE_VERSION_NAME"))
                    versionName = ExtractDefineValue(line);

                line = reader.ReadLine();
            }

            reader.Close();

            Boolean success = (major != "") && (minor != "") && (patch != "");

            if (success)
            {
                // update assembly information file
                String assemblyEntry = String.Format("{0}.{1}.{2}", major, minor, patch);
                ModifyFile(inputManager.MogreAssemblyInfoFile, "AssemblyVersionAttribute.*", String.Format("AssemblyVersionAttribute(\"{0}\")];", assemblyEntry));

                // create message string
                String versionMessage = assemblyEntry;
                if (versionName != "")
                    versionMessage += "  (" + versionName + ")";  // add version name if available

                // print message
                outputManager.Info("Mogre version:  " + versionMessage);

                // save for summary
                outputManager.MogreVersion = versionMessage;

            }
            else
                outputManager.Warning("Unable to detect the Ogre version. So the Mogre version number could be wrong.");


            // update assembly description
            String infoPattern = @"^\[assembly:AssemblyDescriptionAttribute\(.*";
            String infoReplacement = String.Format(
                "[assembly:AssemblyDescriptionAttribute(\"Created by MogreBuilder on {0}-{1:00}-{2:00}\")];",
                DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            ModifyFile(inputManager.MogreAssemblyInfoFile, infoPattern, infoReplacement);

            // update assemlby build type
            String typePattern = @"^\[assembly:AssemblyConfigurationAttribute\(.*";
            String typeReplacement = String.Format(
                "[assembly:AssemblyConfigurationAttribute(\"{0}\")];",
                inputManager.BuildConfiguration);
            ModifyFile(inputManager.MogreAssemblyInfoFile, typePattern, typeReplacement );

        } // Run()



        private string ExtractDefineValue(string line)
        {
            string[] bits = line.Split(' ');

            if (bits != null && bits.Any())
                return bits.Last().Trim().Trim('"');

            return string.Empty;
        }
    }
}
