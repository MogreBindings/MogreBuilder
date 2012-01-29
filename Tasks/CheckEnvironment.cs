using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class CheckEnvironment : Task
    {
        public CheckEnvironment(IOutputManager outputMgr) : base(outputMgr) { }

        public override string ID          { get { return "build:check-env"; } }
        public override string Name        { get { return "Checking environment"; } }
        public override string Description { get { return "Performs checks to verify that the environment is properly set up to run the build."; } }

        public override void Run()
        {
            try
            {
                Cmd(OgreCmake.CMakePath, "--version", null);
                outputManager.Info("Cmake found");
            }
            catch (Exception ex)
            {
                throw new Exception("Can't find cmake in path. Make sure cmake is installed and available in the system path.", ex);
            }

            try
            {
                Cmd("hg", null, null);
                outputManager.Info("Hg found");
            }
            catch (Exception)
            {
                outputManager.Warning("Can't find hg, tasks relying on hg will not run");
            }
        }
    }
}