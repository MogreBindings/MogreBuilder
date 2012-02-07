using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneOgreSource : Task
    {
        public override string ID
        {
            get { return "clone:ogre"; }
        }

        public override string Name
        {
            get { return "Checking out the Ogre source code"; }
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
            if (Directory.Exists(inputManager.OgreRootDirectory) && 
                Directory.EnumerateFileSystemEntries(inputManager.OgreRootDirectory).Any())
            {
                outputManager.Info(
                    string.Format("{0} not empty, assuming Ogre source code checked out already", inputManager.OgreRootDirectory));
            }
            else
            {
                RunCommand("hg", string.Format("clone --verbose {0} -u {1} {2}",
                    inputManager.OgreRepository, inputManager.OgreBranch, inputManager.OgreRootDirectory), null);
            }
        }
    }
}
