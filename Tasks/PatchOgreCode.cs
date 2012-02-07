using System;
using System.IO;
using Mogre.Builder;

namespace Mogre.Builder.Tasks
{
    class PatchOgreCode : Task
    {
        public PatchOgreCode(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "ogre:patch"; } }
        public override string Name        { get { return "Patching Ogre source tree"; } }
        public override string Description { get { return "Applies a bespoke Mogre patch over the Ogre source tree to add certain necessary CLR functionality into Ogre classes."; } }

        public override void Run()
        {
            string patchToolPath = '"' + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\Tools\\pat-ch.exe\"";

            if (RunCommand("hg", "st --modified", inputManager.OgreRootDirectory).Output.Trim() != "")
            {
                outputManager.Info("Ogre code appears to be already patched, skipping patch");
                return;
            }

            try
            {
                RunCommand(patchToolPath, "--version", null);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't execute patch.exe", ex);
            }

            string patchArguments = string.Format(@" -d {0} -p0 --binary --no-backup -i ""{1}""",
                inputManager.OgreRootDirectory, Path.Combine(Directory.GetCurrentDirectory(), inputManager.PatchFile));

            var result = RunCommand(patchToolPath, patchArguments, null);

            if (result.ExitCode != 0)
            {
                outputManager.Warning(result.Output);
                throw new UserException("Patch Failed: " + result.Error);
            }
        }

    }
}