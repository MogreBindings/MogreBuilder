using System;
using System.IO;
using Mogre.Builder;
using System.Reflection;

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
            if (RunCommand("hg", "st --modified", inputManager.OgreRootDirectory).Output.Trim() != "")
            {
                outputManager.Info("Ogre code appears to be already patched, skipping patch");
                return;
            }

            string patchExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "patch.exe");
            var patchFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputManager.ClrPatchFile);
            var result = RunCommand(patchExe, string.Format("-p0 -i \"{0}\"", patchFilePath), inputManager.OgreRootDirectory);

            if (result.ExitCode != 0)
            {
                throw new UserException("Patch Failed: " + result.Error);
            }
        }

    }
}