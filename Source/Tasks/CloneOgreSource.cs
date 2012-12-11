using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneOgreSource : RepositoryTask
    {
        public override string ID
        {
            get { return "clone:ogre"; }
        }

        public override string Name
        {
            get { return "Checking out the Ogre source code (please be very patient)"; }
        }

        public override string Description
        {
            get { return "This task checks out the Ogre repository and downloads the sources to a specified location."; }
        }

        public CloneOgreSource(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {
            PrepareRepository(inputManager.OgreRepository, inputManager.OgreRootDirectory, inputManager.OgreBranch);
        }
    }
}
