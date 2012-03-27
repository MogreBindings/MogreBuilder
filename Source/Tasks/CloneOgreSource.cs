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
            if (Directory.Exists(inputManager.OgreRootDirectory) && 
                Directory.EnumerateFileSystemEntries(inputManager.OgreRootDirectory).Any())
            {
                outputManager.Info(
                    string.Format("{0} not empty, assuming Ogre source code checked out already", inputManager.OgreRootDirectory));
            }
            else
            {
                // make paths bullet-proof  (needed if they contain directories with a space symbol)
                String repositoryPath = Misc.HgPathSecurity(inputManager.OgreRepository);
                String targetPath = Misc.HgPathSecurity(inputManager.OgreRootDirectory);

                CommandResult result = RunCommand("hg", String.Format("clone --verbose {0} -u {1} {2}",
                    repositoryPath, inputManager.OgreBranch, targetPath), null);

                // show warning if error and directory with space symbol
                if ((result.ExitCode != 0) 
                    && (repositoryPath.Contains(" ") || targetPath.Contains(" ")))
                {
                    outputManager.Warning(
                        "Note:  You use a path which contains a space symbol. \n" + 
                        "       Be shure that you use Mercurial version 2.1.1 or newer. \n" +  
                        "       Check it by calling 'hg version' from the command line."
                        );
                }
            }
        }
    }
}
