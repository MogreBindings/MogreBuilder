using System;
using System.IO;
using System.Net;
using Mogre.Builder;
using Ionic.Zip;

namespace Mogre.Builder.Tasks
{
    class OgreDependencies : MsBuildTask
    {
        public OgreDependencies(InputManager inputManager, IOutputManager outputManager, MsBuildManager msBuildMgr) 
            : base(inputManager, outputManager, msBuildMgr) 
        { 
        }

        public override string ID          { get { return "ogre:dependencies"; } }
        public override string Name        { get { return "Handling Ogre dependencies"; } }
        public override string Description { get { return "Downloads the Ogre dependencies if they are not already present, and compiles them if they are not already built."; } }
        
        public override void Run()
        {
            // Check if the dependencies are present
            if (!Directory.Exists(inputManager.DependenciesDirectory))
            {
                GetOgreDependencies();
            }
            else
            {
                outputManager.Info("Ogre dependencies project appears to exist - no need to download");
            }

            // Build the Ogre dependencies if necessary.
            BuildOgreDependencies();
        }

        private void GetOgreDependencies()
        {
            outputManager.Info("Downloading Ogre dependencies");

            try
            {
                bool downloadComplete = false;
                var client = new WebClient();
                Int32 counter = 0;

                client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
                {
                    if (counter++ % 10 == 0)
                        Console.Write(".");
                };

                client.DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                {
                    Console.WriteLine(" ready");
                    downloadComplete = true;
                };

                client.DownloadFileAsync(new Uri(inputManager.DependenciesURL, UriKind.Absolute), inputManager.DependenciesZip);

                while (!downloadComplete)
                    System.Threading.Thread.Sleep(100);
            }
            catch (WebException e)
            {
                throw new UserException("Failed to download Ogre dependencies: " + e.Message);
            }

            try
            {
                outputManager.Info("Unpacking Ogre dependencies");
                var zipFile = new ZipFile(inputManager.DependenciesZip);
                zipFile.ExtractAll(inputManager.OgreRootDirectory);
                zipFile.Dispose();
            }
            catch (Exception e)
            {
                throw new UserException("Failed to unzip Ogre dependencies: " + e.Message);
            }

            File.Delete(inputManager.DependenciesZip);
        }

        private void BuildOgreDependencies()
        {
            outputManager.Info(string.Format("Building Ogre dependencies ({0})", inputManager.BuildConfiguration));
            msBuildManager.Build(inputManager.DependenciesSolutionFile, inputManager.BuildConfiguration, "Win32", "Build");
        }
    }
}