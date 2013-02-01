using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneMogreSource : RepositoryTask
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

        public CloneMogreSource(InputManager inputManager, OutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {
            PrepareRepository(inputManager.MogreRepository, inputManager.TargetDirectory, inputManager.MogreBranch);
        }
    }
}
