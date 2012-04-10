using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder
{

    /// <summary>
    /// This class contains all details which files to copy (or moved) from a source directory to a target directory. <br/>
    /// Options:   <br/>
    /// * Limit the copy task to specific files by patterns (e.g. "*.exe") <br/>
    /// * Create a backup of the prefious files of the target directory <br/>
    /// * Clear the target directory befor copy.
    /// </summary>
    public class FileCopyTask
    {
        /// <summary>
        /// Define all details which files to copy (or move) from a source directory to a target directory.
        /// </summary>
        /// <param name="sourceDirectory">Path of the source directory</param>
        /// <param name="targetDirectory">Path of the destination directory</param>
        public FileCopyTask(String sourceDirectory, String targetDirectory)
        {
            this.SourceDirectory = sourceDirectory;
            this.TargetDirectory = targetDirectory;

            // default settings
            this.FilePattern = "*";
            this.TargetClearType = ClearType.KeepAll;
            this.CreateBackup = false;
            this.Counter = 0;
            this.PrintDots = false;
        }



        /// <summary>
        /// Define all details which files to copy (or move) from a source directory to a target directory.  <br/>
        /// The filePattern parameter can be used to copy only specific files. 
        /// If filePattern is empty (""), all files will be copied.
        /// </summary>
        /// <param name="sourceDirectory">Path of the source directory</param>
        /// <param name="targetDirectory">Path of the destination directory</param>
        /// <param name="filePattern">Filter to define, which files to copy. (e.g. "*.exe") If empty, all files will be copied.</param>
        public FileCopyTask(String sourceDirectory, String targetDirectory, String filePattern)
            : this(sourceDirectory, targetDirectory)
        {
            // NOTE: Before call of this code, the content of the basis constructor (above) will be processed.
            this.FilePattern = filePattern;
        }



        public String SourceDirectory { get; set; }
        public String TargetDirectory { get; set; }

        /// <param name="createBackup">If true, it will be created a backup of all previous files in the target directory. (Default: false)</param>
        public Boolean CreateBackup { get; set; }

        /// <param name="clearType">An option if the target directory should be cleared before. (Default: KeepAll files)</param>
        public ClearType TargetClearType { get; set; }

        /// <summary>If enabled, for each copied file a dot "." will be printed. (Default: false)</summary>
        public Boolean PrintDots { get; set; }


        /// <summary>Note: For multiple patterns use space as seperator " ". Example: "*.cpp *.h" - If empty, all files will be copied.</summary>
        public String FilePattern { get; set; }

        /// <summary>Amount of files, which were copied or moved.</summary>
        public UInt32 Counter { get; private set; }

        /// <summary>
        /// Return a list of all single patterns. If no filePattern is defined, it returns "*". 
        /// The returned patterns are compatible for the method <c>Directory.GetFiles()</c>.
        /// </summary>
        public String[] GetAllPatterns()
        {
            String[] allPatterns = Regex.Split(FilePattern, @"[ ]+");

            if (allPatterns.Length == 0)
                return new String[] { "*" };
            else
                return allPatterns;
        }


        /// <summary>Copy all files now. (Or limited by patterns.)</summary>
        public void DoCopy()
        {
            CopyMove(false, this.CreateBackup, this.TargetClearType);
        }


        /// <summary>Move all files now. (Or limited by patterns.)</summary>
        public void DoMove()
        {
            CopyMove(true, this.CreateBackup, this.TargetClearType);
        }

        
        
        /// <summary>Perform the copy or move process</summary>
        private void CopyMove(Boolean doMove, Boolean createBackup, ClearType targetClearType)
        {
            // check if valide source directory
            if (Directory.Exists(SourceDirectory) == false)
                return;

            // create backup if needed
            if (createBackup)
                BackupOldFiles(TargetDirectory, PrintDots);


            // delete old files of target directory if needed
            if (targetClearType != ClearType.KeepAll)
            {
                List<String> killingList = new List<String>();

                // create deletion list
                if (targetClearType == ClearType.DeleteAll)
                {
                    // all files
                    killingList.AddRange(Directory.GetFiles(TargetDirectory));
                }
                else
                {
                    // files by patterns
                    foreach (String pattern in GetAllPatterns())
                        killingList.AddRange(Directory.GetFiles(TargetDirectory, pattern));
                }

                // deletion
                foreach (String file in killingList)
                {
                    RemoveWriteProtection(file);
                    File.Delete(file);
                }
            }


            // copy / move
            foreach (String pattern in GetAllPatterns())
                foreach (String sourceFile in Directory.GetFiles(SourceDirectory, pattern))
                {
                    String targetFile = Path.Combine(TargetDirectory, Path.GetFileName(sourceFile));
                    RemoveWriteProtection(targetFile);
                    File.Copy(sourceFile, targetFile, true);

                    // move or copy?
                    if (doMove)
                        File.Delete(sourceFile);

                    // print dots
                    if (PrintDots)
                        Console.Write(".");
                    Counter++;
                }

            // print linebreak after dots
            if (PrintDots)
                Console.WriteLine();

        } // CopyMove()





        public enum ClearType
        {
            DeleteAll,
            DeleteByPatterns,
            KeepAll
        }





        /// <summary>
        /// Copy files with advanded options.
        /// </summary>
        /// <param name="sourceDirectory">Path of the source directory</param>
        /// <param name="targetDirectory">Path of the destination directory</param>
        /// <param name="filePattern">Filter to define, which files to copy. (e.g. "*.exe") If empty, all files will be copied.</param>
        /// <param name="createBackup">If true, it will be created a backup of all previous files in the target directory.</param>
        /// <param name="targetClearType">An option if the target directory should be cleared before. </param>
        /// <param name="printDots">If enabled, for each copied file a dot "." will be printed. (Default: false)</param>
        public static UInt32 DoCopy(String sourceDirectory, String targetDirectory, String filePattern, Boolean createBackup, ClearType targetClearType, Boolean printDots)
        {
            FileCopyTask task = new FileCopyTask(sourceDirectory, targetDirectory, filePattern);
            task.CreateBackup = createBackup;
            task.TargetClearType = targetClearType;
            task.PrintDots = printDots;
            task.DoCopy();
            return task.Counter;
        }



        /// <summary>
        /// Move files with advanded options.
        /// </summary>
        /// <param name="sourceDirectory">Path of the source directory</param>
        /// <param name="targetDirectory">Path of the destination directory</param>
        /// <param name="filePattern">Filter to define, which files to move. (e.g. "*.exe") If empty, all files will be copied.</param>
        /// <param name="createBackup">If true, it will be created a backup of all previous files in the target directory.</param>
        /// <param name="targetClearType">An option if the target directory should be cleared before. </param>
        /// <param name="printDots">If enabled, for each copied file a dot "." will be printed. (Default: false)</param>
        public static UInt32 DoMove(String sourceDirectory, String targetDirectory, String filePattern, Boolean createBackup, ClearType targetClearType, Boolean printDots)
        {
            FileCopyTask task = new FileCopyTask(sourceDirectory, targetDirectory, filePattern);
            task.CreateBackup = createBackup;
            task.TargetClearType = targetClearType;
            task.PrintDots = printDots;
            task.DoMove();
            return task.Counter;
        }








        /// <summary>
        /// Copy all files of a directory to the subdirectory "backup". 
        /// If the directory is invalide, nothing happens. (No exception, no warning.)
        /// </summary>
        /// <param name="directory">Directory where to create a backup</param>
        /// <param name="printDots">If enabled, for each copied file a dot "." will be printed.</param>
        public static void BackupOldFiles(String directory, Boolean printDots)
        {
            if (Directory.Exists(directory) == false)
                return;

            String errorMessage = "Error at file backup:   ";

            try
            {
                String bakDir = Path.Combine(directory, "backup");
                String[] allFiles = Directory.GetFiles(directory);

                // create backup directory if needed
                if (allFiles.Length > 0)
                    Directory.CreateDirectory(bakDir);

                // copy all files
                foreach (String file in allFiles)
                {
                    String bakFileTarget = Path.Combine(bakDir, Path.GetFileName(file));
                    RemoveWriteProtection(bakFileTarget);
                    File.Copy(file, bakFileTarget, true);

                    // print dots
                    if (printDots)
                        Console.Write("*");
                }
            }
            catch (IOException e)
            {
                throw new Exception(errorMessage + e.Message, e.InnerException);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new Exception(errorMessage + e.Message, e.InnerException);
            }
            catch (ArgumentNullException e)
            {
                throw new Exception(errorMessage + e.Message, e.InnerException);
            }


        } // BackupOldFiles()





        /// <summary>
        /// Remove file property "read only". 
        /// It's for the special case that a copied file contains this flag. So the backup files can't be modified.
        /// </summary>
        /// <param name="file">File name (Recommend: Include full path)</param>
        public static void RemoveWriteProtection(String file)
        {
            if (File.Exists(file))
            {
                FileAttributes attributes = File.GetAttributes(file);
                
                // check if "read only"
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes = attributes ^ FileAttributes.ReadOnly;  // invert flag
                    File.SetAttributes(file, attributes);
                }
            }
        } // RemoveWriteProtection()


    } // class FileCopyTask


} // namespace
