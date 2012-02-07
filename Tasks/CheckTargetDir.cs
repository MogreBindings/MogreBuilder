using System;
using System.IO;

namespace Mogre.Builder.Tasks
{
    /// <summary>
    /// Looks for the Ogre source directory.
    /// </summary>
    class CheckTargetDir : Task
    {
        public CheckTargetDir(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override string ID          { get { return "build:check-target"; } }
        public override string Name        { get { return "Checking target directory"; } }
        public override string Description { get { return "Checks to see that the target directory is a valid Mogre checkout. This is an internal task that is always executed."; } }

        public override void Run()
        {
            if (inputManager.TargetDirectory != "")
            {
                try
                {
                    Directory.SetCurrentDirectory(inputManager.TargetDirectory);
                }
                catch (ArgumentException)
                {
                    throw new UserException("Target directory is invalid or does not exist.");
                }
            }

            var found = Array.FindAll<string>(
                Directory.GetFileSystemEntries("."),
                delegate(string entry)
                {
                    return (entry == @".\BUILD" || entry == @".\Codegen" || entry == @".\Main");
                }
            ).Length;

            if (found != 3)
                throw new UserException("Target directory does not appear to be the root of a Mogre code tree.");

            outputManager.Info(string.Format("Target Directory: {0}", Directory.GetCurrentDirectory()));
        }
    }
}