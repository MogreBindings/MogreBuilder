using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneDependenciesRepository : RepositoryTask
    {
        public override string ID
        {
            get { return "clone:ogre-dependencies"; }
        }

        public override string Name
        {
            get { return "Checking out the source code of Ogre dependencies"; }
        }

        public override string Description
        {
            get { return "This task checks out the Ogre dependencies repository and downloads the sources to a specified location."; }
        }

        public CloneDependenciesRepository(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {
            PrepareRepository(inputManager.DependenciesRepository, inputManager.DependenciesDirectory);
        }
    }
}
