using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneDependenciesRepository : Task
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
            // create directory if needed
            if (Directory.Exists(inputManager.DependenciesDirectory) == false)
            {
                Directory.CreateDirectory(inputManager.DependenciesDirectory);
                outputManager.Info("Created directory:  " + inputManager.DependenciesDirectory);
            }

            // check if still cloned
            if (Directory.EnumerateFileSystemEntries(inputManager.DependenciesDirectory).Any())
            {
                // no cloning needed
                outputManager.DisplayMessage(String.Format(
                    "\nThe directory '{0}' is not empty, assuming Ogre dependencies source code checked out already",
                    inputManager.DependenciesDirectory), ConsoleColor.White);
            }
            else // do clone
            {
                // make paths bullet-proof  (needed if they contain directories with a space symbol)
                String repositoryPath = Misc.HgPathSecurity(inputManager.DependenciesRepository);
                String targetPath = Misc.HgPathSecurity(inputManager.DependenciesDirectory);

                // clone "default" branch
                RunCommand("hg", string.Format("clone --verbose {0} -u {1} {2}",
                    repositoryPath, "default", targetPath), null);
            }
        }
    }
}
