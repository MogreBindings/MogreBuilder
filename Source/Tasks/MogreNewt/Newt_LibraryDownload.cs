using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class NewtonLibraryDownload : Task
    {
        public override string ID
        {
            get { return "newton:LibraryDownload"; }
        }

        public override string Name
        {
            get { return "Download the Newton library"; }
        }

        public override string Description
        {
            get { return "This task downloads the Newton library (if needed)."; }
        }

        public NewtonLibraryDownload(InputManager inputManager, OutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {

            outputManager.Warning(
                "This functionality is currently not implemented. \n" + 
                " --> Copy the Newton binaries and header files manually to the directory: \n" + 
                "     " + Path.Combine(inputManager.TargetDirectory, inputManager.NewtonInputDirectory_NewtonLibrary));

            // TODO: 
            //  * Either grabb/extract/copy the needed files from the Newton website  (about 60 MB)
            //                  http://code.google.com/p/newton-dynamics/downloads/list
            //  * OR put the needed files as 7z download archive to the Mogre bitbucket page.  (about 3 MB for 32bit files)

        }

    } //  class NewtonLibraryDownload
}
