using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class CheckEnvironment : Task
    {
        public CheckEnvironment(OutputManager outputMgr) : base(outputMgr) { }

        public override string ID          { get { return "build:check-env"; } }
        public override string Name        { get { return "Checking environment"; } }
        public override string Description { get { return "Performs checks to verify that the environment is properly set up to run the build."; } }

        public override void Run()
        {
            try
            {
                Cmd("cmake --version");
                outputMgr.Info("Cmake found");
            }
            catch (Exception)
            {
                throw new UserException("Can't find cmake in path. Make sure cmake is installed and available in the system path.");
            }

            try
            {
                Cmd("hg");
                outputMgr.Info("Hg found");
            }
            catch (Exception)
            {
                outputMgr.Warn("Can't find hg, tasks relying on hg will not run");
            }
        }
    }
}