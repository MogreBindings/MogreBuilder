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

            string mogreVersion = string.Format("{0}.{1}.{2}", major, minor, patch);

            if (!string.IsNullOrWhiteSpace(mogreVersion))
                ModifyFile(inputManager.MogreAssemblyInfoFile, "AssemblyVersionAttribute.*", string.Format("AssemblyVersionAttribute(\"{0}\")];", mogreVersion));
            else
                outputManager.Warning("Unable to update Mogre version, the version number could be wrong");
        }

        private string ExtractDefineValue(string line)
        {
            string[] bits = line.Split(' ');

            if (bits != null && bits.Any())
                return bits.Last().Trim().Trim('"');

            return string.Empty;
        }
    }
}
