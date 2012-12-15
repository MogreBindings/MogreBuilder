using System;
using System.IO;

namespace Mogre.Builder
{
    abstract class RepositoryTask : Task
    {
        public RepositoryTask(InputManager inputManager, IOutputManager outputManager)
            : base(inputManager, outputManager)
        {
            
        }

        protected void PrepareRepository(string repository, string target, string branch = "default")
        {
            // make paths bullet-proof  (needed if they contain directories with a space symbol)
            String repositoryPath = Misc.HgPathSecurity(repository);
            String targetPath = Misc.HgPathSecurity(target);

            // if repository is already cloned
            if (Directory.Exists(Path.Combine(target, ".hg")))
            {
                if (!inputManager.Option_NoUpdate)
                {
                    // pull latest changes from remote repository
                    if (RunCommand("hg", string.Format("pull -R {0} {1}", targetPath, repository), null).ExitCode != 0)
                    {
                        throw new Exception(string.Format("Could not pull to repository {0}", target));
                    }
                }
                else
                {
                    outputManager.Info(string.Format("Updating repositories was disabled. Using latest local commit instead."));
                }

                // update working copy to latest changeset and discard uncommited changes
                if (RunCommand("hg", string.Format("update --clean -R {0} {1}", targetPath, branch), null).ExitCode != 0)
                {
                    throw new Exception(string.Format("Could not update repository {0}", target));
                }

                // remove all untracked, temporary files (using mercurial's purge extension)
                if (RunCommand("hg", string.Format("--config extensions.hgext.purge= purge --all -R {0}", targetPath), null).ExitCode != 0)
                {
                    throw new Exception(string.Format("Could not purge to repository {0}", target));
                }
            }
            else
            {
                // create directory if needed
                if (!Directory.Exists(target))
                {
                    Directory.CreateDirectory(target);
                    outputManager.Info("Created directory:  " + target);
                }

                if (RunCommand("hg", string.Format("clone --verbose {0} -u {1} {2}", repositoryPath, branch, targetPath), null).ExitCode != 0)
                {
                    throw new Exception(string.Format("Could not clone repository {0} to {1}", repository, target));
                }
            }
        }
    }
}