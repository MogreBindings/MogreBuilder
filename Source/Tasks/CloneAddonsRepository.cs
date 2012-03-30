using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class CloneAddonsRepository : Task
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

        public CloneAddonsRepository(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {
            // check if Mercurial is installed
            try
            {
                RunCommand("hg", "--version", null);
                outputManager.Info(""); // empty line for better overview
            }
            catch (Exception ex)
            {
                throw new Exception("Can't find hg in path. Make sure hg is installed and available in the system path.", ex);
            }

            // create directory if needed
            if (Directory.Exists(inputManager.MogreAddonsDirectory) == false)
            {
                Directory.CreateDirectory(inputManager.MogreAddonsDirectory);
                outputManager.Info("Created directory:  " + inputManager.MogreAddonsDirectory);
            }

            if (Directory.EnumerateFileSystemEntries(inputManager.MogreAddonsDirectory).Any())
            {
                outputManager.DisplayMessage(String.Format(
                    "\nThe directory '{0}' is not empty, assuming Mogre Add-ons source code checked out already", 
                    inputManager.MogreAddonsDirectory),ConsoleColor.White);
            }
            else
            {
                // make paths bullet-proof  (needed if they contain directories with a space symbol)
                String repositoryPath = Misc.HgPathSecurity(inputManager.MogreAddonsRepository);
                String targetPath = Misc.HgPathSecurity(inputManager.MogreAddonsDirectory);

                // clone "default" branch
                RunCommand("hg", string.Format("clone --verbose {0} -u {1} {2}",
                    repositoryPath, "default", targetPath), null);
            }
        }
    }
}
