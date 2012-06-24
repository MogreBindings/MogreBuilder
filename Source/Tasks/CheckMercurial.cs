using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    class CheckMercurial : Task
    {
        public CheckMercurial(InputManager inputManager, IOutputManager outputManager) 
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "build:check-mercurial"; } }
        public override string Name        { get { return "Checking mercurial"; } }
        public override string Description { get { return "Verifies that mercurial is available."; } }

        public override void Run()
        {
            try
            {
                RunCommand("hg", "--version", null);
            }
            catch (Exception ex)
            {
                throw new Exception("Can't find hg in path. Make sure hg is installed and available in the system path.", ex);
            }
        }
    }
}