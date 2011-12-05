using System;
using System.IO;
using Mogre.Builder;

namespace Mogre.Builder.Tasks
{
    class PatchOgreCode : Task
    {
        public PatchOgreCode(OutputManager outputMgr) : base(outputMgr) { }

        public override string ID          { get { return "ogre:patch"; } }
        public override string Name        { get { return "Patching Ogre source tree"; } }
        public override string Description { get { return "Applies a bespoke Mogre patch over the Ogre source tree to add certain necessary CLR functionality into Ogre classes."; } }

        public override void Run()
        {
            var patch = '"' + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\pat-ch.exe\"";

            if (Cmd("hg st --modified", @"Main\OgreSrc\ogre").Output.Trim() != "")
            {
                outputMgr.Info("Ogre code appears to be already patched, skipping patch");
                return;
            }

            try
            {
                Cmd(patch + " --version");
            }
            catch (Exception)
            {
                throw new UserException("Can't execute patch.exe");
            }

            var result = Cmd(string.Format(patch + @" -d Main\OgreSrc\ogre -p0 --binary --no-backup -i ""{0}""", Directory.GetCurrentDirectory() + @"\Main\Ogre Patches\58266f25ccd2.patch"));
            if (result.ExitCode != 0)
                throw new UserException("Patch Failed: " + result.Error);
        }

    }
}