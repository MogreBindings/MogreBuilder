using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneAddonsRepository : RepositoryTask
    {
        public override string ID
        {
            get { return "clone:mogre-addons"; }
        }

        public override string Name
        {
            get { return "Checking out the source code of the Mogre Add-ons (please be patient)"; }
        }

        public override string Description
        {
            get { return "This task checks out the Mogre Add-ons repository and downloads the sources to a specified location."; }
        }

        public CloneAddonsRepository(InputManager inputManager, OutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {
            PrepareRepository(inputManager.MogreAddonsRepository, inputManager.MogreAddonsDirectory);
        }
    }
}
