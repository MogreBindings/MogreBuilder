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
            if (RunCommand("hg", "st --modified", inputManager.OgreRootDirectory).Output.Trim() != "")
            {
                outputManager.Info("Ogre code appears to be already patched, skipping patch");
                return;
            }

            var patchFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputManager.PatchFile);
            var result = RunCommand("hg", string.Format("import --strip 0 --no-commit \"{0}\"", patchFilePath), inputManager.OgreMainDirectory);

            if (result.ExitCode != 0)
            {
                throw new UserException("Patch Failed: " + result.Error);
            }
        }

    }
}