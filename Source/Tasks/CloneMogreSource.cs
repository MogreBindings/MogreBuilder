using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneMogreSource : Task
    {
        public override string ID
        {
            get { return "clone:mogre"; }
        }

        public override string Name
        {
            get { return "Checking out the source code of Mogre (please be patient)"; }
        }

        public override string Description
        {
            get { return "This task checks out the Mogre repository and downloads the sources to a specified location."; }
        }

        public CloneMogreSource(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {
            if (Directory.EnumerateFileSystemEntries(inputManager.TargetDirectory).Any())
            {
                outputManager.Info(
                    string.Format("{0} not empty, assuming Mogre source code checked out already", inputManager.TargetDirectory));
            }
            else
            {
                RunCommand("hg", string.Format("clone --verbose {0} -u {1} {2}",
                    inputManager.MogreRepository, inputManager.MogreBranch, inputManager.TargetDirectory), null);
            }
        }
    }
}
