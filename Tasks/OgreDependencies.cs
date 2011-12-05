using System;
using System.IO;
using System.Net;
using Mogre.Builder;
using Ionic.Zip;

namespace Mogre.Builder.Tasks
{
    class OgreDependencies : MsBuildTask
    {
        public OgreDependencies(OutputManager outputMgr, MsBuildManager msBuildMgr) : base(outputMgr, msBuildMgr) { }

        public override string ID          { get { return "ogre:dependencies"; } }
        public override string Name        { get { return "Handling Ogre dependencies"; } }
        public override string Description { get { return "Downloads the Ogre dependencies if they are not already present, and compiles them if they are not already built."; } }
        
        public override void Run()
        {
            // Check if the dependencies are present
            if (!Directory.Exists(@"Main\OgreSrc\ogre\Dependencies"))
            {
                GetOgreDependencies();
            }
            else
            {
                outputMgr.Info("Ogre dependencies project appears to exist - no need to download");
            }

            // Build the Ogre dependencies if necessary.
            BuildOgreDependencies();
        }

        private void GetOgreDependencies()
        {
            outputMgr.Info("Downloading Ogre dependencies");

            try
            {
                bool downloadComplete = false;
                var client = new WebClient();
                client.DownloadProgressChanged += delegate(object sender, DownloadProgressChangedEventArgs e)
                {
                    Console.Write(".");
                };
                client.DownloadFileCompleted += delegate(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                {
                    Console.WriteLine(".");
                    downloadComplete = true;
                };
                client.DownloadFileAsync(
                    new Uri("http://surfnet.dl.sourceforge.net/project/ogre/ogre-dependencies-vc%2B%2B/1.7/OgreDependencies_MSVC_20100501.zip", UriKind.Absolute),
                    @"Main\OgreSrc\ogre\Dependencies.zip");

                while (!downloadComplete)
                    System.Threading.Thread.Sleep(100);
            }
            catch (WebException e)
            {
                throw new UserException("Failed to download Ogre dependencies: " + e.Message);
            }

            try
            {
                outputMgr.Info("Unpacking Ogre dependencies");
                var zipFile = new ZipFile(@"Main\OgreSrc\ogre\Dependencies.zip");
                zipFile.ExtractAll(@"Main\OgreSrc\ogre");
                zipFile.Dispose();
            }
            catch (Exception e)
            {
                throw new UserException("Failed to unzip Ogre dependencies: " + e.Message);
            }

            File.Delete(@"Main\OgreSrc\ogre\Dependencies.zip");
        }

        private void BuildOgreDependencies()
        {
            outputMgr.Info("Building Ogre dependencies (Debug)");
            msBuildMgr.Build(@"Main\OgreSrc\ogre\Dependencies\src\OgreDependencies.VS2010.sln", "Debug", "Win32", "Build");
            outputMgr.Info("Building Ogre dependencies (Release)");
            msBuildMgr.Build(@"Main\OgreSrc\ogre\Dependencies\src\OgreDependencies.VS2010.sln", "Release", "Win32", "Build");
        }
    }
}