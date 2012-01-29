using System;
using System.IO;
using Mogre.Builder;

namespace Mogre.Builder.Tasks
{
    class PatchOgreCode : Task
    {
        public PatchOgreCode(IOutputManager outputMgr) : base(outputMgr) { }

        public override string ID          { get { return "ogre:patch"; } }
        public override string Name        { get { return "Patching Ogre source tree"; } }
        public override string Description { get { return "Applies a bespoke Mogre patch over the Ogre source tree to add certain necessary CLR functionality into Ogre classes."; } }

        public override void Run()
        {
            var patch = '"' + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\pat-ch.exe\"";

            if (Cmd("hg", "st --modified", @"Main\OgreSrc\ogre").Output.Trim() != "")
            {
                outputManager.Info("Ogre code appears to be already patched, skipping patch");
                return;
            }

            try
            {
                Cmd(patch, "--version", null);
            }
            catch (Exception)
            {
                throw new UserException("Can't execute patch.exe");
            }

            string patchArguments = string.Format(@" -d Main\OgreSrc\ogre -p0 --binary --no-backup -i ""{0}""", 
                Directory.GetCurrentDirectory() + @"\Main\Ogre Patches\58266f25ccd2.patch");

            var result = Cmd(patch, patchArguments, null);

            if (result.ExitCode != 0)
            {
                outputManager.Warning(result.Output);
                throw new UserException("Patch Failed: " + result.Error);
            }
        }

    }
}