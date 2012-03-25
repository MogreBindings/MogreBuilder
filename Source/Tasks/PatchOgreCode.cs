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

            String patchExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "patch.exe");
            String patchFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputManager.ClrPatchFile);
            CommandResult result = RunCommand(patchExe, String.Format("-p0 -i \"{0}\"", patchFilePath), inputManager.OgreRootDirectory);

            if (result.ExitCode != 0)
            {

                // throw new UserException("Ogre patch Failed: " + result.Error);

                // prepare message
                String message = "Ogre patch failed";
                if (result.Error == "")
                    message += "!    (Happens when the files are still patched by a previous run.)";
                else
                    message += ":  " + result.Error;

                // print message
                outputManager.Warning(message);

            }
        } // Run()

    }
}