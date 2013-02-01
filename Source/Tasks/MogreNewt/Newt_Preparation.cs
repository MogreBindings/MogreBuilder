using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class NewtonPrepatation : Task
    {
        public override string ID
        {
            get { return "newton:preparation"; }
        }

        public override string Name
        {
            get { return "Prepare building of add-on MogreNewt"; }
        }

        public override string Description
        {
            get { return "This task checks directories and and grabs all needed depency files."; }
        }

        public NewtonPrepatation(InputManager inputManager, OutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {
            outputManager.Info("Check directories ...");
            List<String> missingDirectoryList = new List<String>();

            // check directory of the MogreNewt wrapper
            if (Directory.Exists(inputManager.NewtonMainDirectory) == false)
            {
                outputManager.Error(String.Format(
                    "Can't find directory of the MogreNewt project. \n" + 
                    "    Estimated location:  {0} \n",
                    inputManager.NewtonMainDirectory));
                throw new Exception("Application stopped");
            }

            // check directory of the Newton library
            if (Directory.Exists(inputManager.NewtonDepencySourceDir_NewtonLibrary) == false)
            {
                String message;
                if (inputManager.NewtonDepencySourceDir_NewtonLibrary == "")
                    // nothing defined
                    message = "The directory of the Newton library is not defined! \n" +
                              " --> Either add the path to the parameter 'NewtonDepencySourceDir_NewtonLibrary' in the config file \n" + 
                              "     OR put the files manually to:  " + inputManager.NewtonInputDirectory_NewtonLibrary;
                else
                    // path not found
                    message = "Can't find directory of the Newton library at location: \n    " +
                              inputManager.MogreAddonsDirectory;
                outputManager.Warning(message + "\n");
            }


            // directories where to find the needed depency files
            String[] depencyDirectories = 
            { 
                inputManager.NewtonDepencySourceDir_Mogre_binary,
                inputManager.NewtonDepencySourceDir_Ogre_headers,
                inputManager.NewtonDepencySourceDir_Ogre_headers_Threading,
                inputManager.NewtonDepencySourceDir_Ogre_headers_Win32,
                inputManager.NewtonDepencySourceDir_OgreMain_binaries,
                inputManager.NewtonDepencySourceDir_OgreMain_headers
            };

            // check if they exist
            foreach (String directory in depencyDirectories)
                if (Directory.Exists(directory) == false)
                    missingDirectoryList.Add(directory);

            // display warnings if directory not found
            if (missingDirectoryList.Count > 0)
            {
                outputManager.Warning("Can't find these directories, which contain the needed depency files:");
                foreach (String dir in missingDirectoryList)
                    outputManager.Warning("    " + dir);
                outputManager.Warning(String.Format(
                    "--> Possible solutions: \n" +
                    "     * Check if Mogre was build successfully. \n" +
                    "     * Check if the paths are correct. \n" +
                    "     * OR copy the needed files manually to directory:  \n" +
                    "          {0} \n ", inputManager.NewtonMainDirectory));
            }
            else
            {
                outputManager.Info("OK, all directories of the Ogre/Mogre depency files were found.");
            }

            // reset warning list
            missingDirectoryList.Clear();


            // directories where to copy the needed depency files
            String[] inputDirectories = 
            { 
                inputManager.NewtonInputDirectory_Mogre_binary_debug,
                inputManager.NewtonInputDirectory_Mogre_binary_release,
                inputManager.NewtonInputDirectory_NewtonLibrary,
                inputManager.NewtonInputDirectory_Ogre_headers,
                inputManager.NewtonInputDirectory_Ogre_headers_Threading,
                inputManager.NewtonInputDirectory_Ogre_headers_Win32,
                inputManager.NewtonInputDirectory_OgreMain_binaries,
                inputManager.NewtonInputDirectory_OgreMain_headers
            };

            // check if they exist
            foreach (String directory in inputDirectories)
                if (Directory.Exists(directory) == false)
                    missingDirectoryList.Add(directory);

            // create input directories if not found
            if (missingDirectoryList.Count > 0)
            {
                outputManager.Info("Create missing target directories (where to copy the depency files) ... ");
                foreach (String dir in missingDirectoryList)
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                    }
                    catch (Exception e)
                    {
                        outputManager.Error("    " + dir);
                        throw new Exception("    --> Error at directory creation: \n        " + e.Message);
                    }
                    // print directory name
                    outputManager.Info("    " + dir);
                }

                outputManager.Info("");        // print empty line (for better overview)
                missingDirectoryList.Clear(); 
            }
            else
            {
                outputManager.Info("OK, all target directories for depency files were found.");
            }



            //--- copy all needed depency files ---

            Boolean printDots = true;
            Boolean createBackup = true;

            outputManager.DisplayMessage("\nCopy all needed files", ConsoleColor.White);

            if (createBackup == true)
                outputManager.Info("Backup creation for previous files is enabled.");
            Console.WriteLine();
            

            // create task list
            List<FileCopyTask> fileCopyTasks = new List<FileCopyTask>() 
            { 
                new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_Ogre_headers,
                    inputManager.NewtonInputDirectory_Ogre_headers,
                    "*.h"),
                new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_Ogre_headers_Threading,
                    inputManager.NewtonInputDirectory_Ogre_headers_Threading,
                    "*.h"),
                new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_Ogre_headers_Win32,
                    inputManager.NewtonInputDirectory_Ogre_headers_Win32,
                    "*.h"),
                new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_OgreMain_binaries,
                    inputManager.NewtonInputDirectory_OgreMain_binaries,
                    "OgreMain.*"),
                new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_OgreMain_headers,
                    inputManager.NewtonInputDirectory_OgreMain_headers,
                    "*.*"),
            };


            // choose target directory of the Mogre binary files
            if (inputManager.BuildConfiguration.ToLower() == "release")
            {
                
                fileCopyTasks.Add(new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_Mogre_binary,
                    inputManager.NewtonInputDirectory_Mogre_binary_release,
                    "Mogre.*"));

                // clear "debug" directory to avoid problems  (before: create backup)
                FileCopyTask.BackupOldFiles(inputManager.NewtonInputDirectory_Mogre_binary_debug, printDots);
                foreach (String file in Directory.GetFiles(inputManager.NewtonInputDirectory_Mogre_binary_debug, 
                    "Mogre.*"))
                    File.Delete(file);
            }
            else
            {
                fileCopyTasks.Add(new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_Mogre_binary,
                    inputManager.NewtonInputDirectory_Mogre_binary_debug,
                    "Mogre.*"));

                // clear "release" directory to avoid problems  (before: create backup)
                FileCopyTask.BackupOldFiles(inputManager.NewtonInputDirectory_Mogre_binary_release, printDots);
                foreach (String file in Directory.GetFiles(inputManager.NewtonInputDirectory_Mogre_binary_release,
                    "Mogre.*"))
                    File.Delete(file);
            }


            // check if Newton library is available
            if (inputManager.NewtonDepencySourceDir_NewtonLibrary != "")
                fileCopyTasks.Add(new FileCopyTask(
                    inputManager.NewtonDepencySourceDir_NewtonLibrary,
                    inputManager.NewtonInputDirectory_NewtonLibrary,
                    "*.lib *.h"));


            UInt32 copyCounter = 0;
            Boolean noFileWarning = false;

            // do copy
            foreach (FileCopyTask task in fileCopyTasks)
            {
                // create a backup of the previous files in target directory
                task.CreateBackup = createBackup;

                // delete all files, where the pattern matches
                //   --> Avoid conficts with old files
                //   --> Other files (e.g. text files) will not be deleted
                task.TargetClearType = FileCopyTask.ClearType.DeleteByPatterns;
                task.PrintDots = printDots;

                Console.WriteLine("Copy from:  " + task.SourceDirectory);
                Console.WriteLine("File pattern:  " + task.FilePattern);

                task.DoCopy();

                // check if failed
                if (task.Counter == 0)
                {
                    // print information
                    outputManager.Warning(String.Format(
                        "No files found in depency directory  \n" +
                        "    Source:  {0}", task.SourceDirectory));
                    outputManager.Info(String.Format(
                        "    Target:  {0} \n" +
                        "    File pattern:  {1} ",  task.TargetDirectory, task.FilePattern
                        ));
                    noFileWarning = true;
                }
                
                copyCounter += task.Counter;
            } // foreach

            // print notice
            if (noFileWarning)
                outputManager.Warning(" --> Are you shure that Mogre was build successfully before? ");

            // print amout of copied files
            outputManager.Info(String.Format("\n{0} files were copied.", copyCounter));




        } //  Run()




    } //  class NewtonPrepatation





}
