using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class CheckEnvironment : Task
    {
        public CheckEnvironment(InputManager inputManager, IOutputManager outputManager) 
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "build:check-env"; } }
        public override string Name        { get { return "Checking environment"; } }
        public override string Description { get { return "Performs checks to verify that the environment is properly set up to run the build."; } }

        public override void Run()
        {
            // check if CMake is installed
            try
            {
                RunCommand(inputManager.CMakeExecutable, "--version", null);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't find cmake in path. Make sure cmake is installed and available in the system path.", ex);
            }
        }
    }
}