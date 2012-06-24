using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class CheckTools : Task
    {
        public CheckTools(InputManager inputManager, IOutputManager outputManager) 
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "build:check-toos"; } }
        public override string Name        { get { return "Checking for tools"; } }
        public override string Description { get { return "Verify that Mercuial and CMake are available, because they are needed to peform the build."; } }

        public override void Run()
        {
            // check if Mercurial is installed
            try
            {
                RunCommand("hg", "--version", null);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't find hg in path. Make sure hg is installed and available in the system path.", ex);
            }

            outputManager.Info(" "); // add empty line

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