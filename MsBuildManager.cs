using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace Mogre.Builder
{
    class MsBuildManager : ILogger
    {
        private List<string>  visited;
        private IOutputManager outputMgr;
        
        public MsBuildManager(IOutputManager outputMgr)
        {
            this.outputMgr = outputMgr;
        }

        public BuildResult Build(string projectFile, string configuration, string platform, string[] targets)
        {
            var buildParams = new BuildParameters(new ProjectCollection());
            buildParams.Loggers = new ILogger[] { this };

            var buildResult = BuildManager.DefaultBuildManager.Build(
                buildParams,
                new BuildRequestData(
                    projectFile,
                    new Dictionary<string, string>()
                    {
                        {"Configuration", configuration},
                        {"Platform",      platform},
                    },
                    null,
                    targets,
                    null)
            );

            if (buildResult.OverallResult == BuildResultCode.Failure)
                throw new UserException("Failed to build " + projectFile);

            return buildResult;
        }

        public BuildResult Build(string projectFile, string configuration, string platform, string target)
        {
            return Build(projectFile, configuration, platform, new string[] { target });
        }

        public void Initialize(IEventSource eventSource)
        {
            eventSource.BuildStarted   += OnBuildStarted;
            eventSource.ProjectStarted += OnProjectStarted;
            eventSource.ErrorRaised    += OnErrorRaised;
            eventSource.TaskStarted    += OnTaskStarted;
            eventSource.BuildFinished  += OnBuildFinished;
        }

        private void OnBuildStarted(object sender, BuildStartedEventArgs e)
        {
            visited = new List<string>();
        }

        private void OnProjectStarted(object sender, ProjectStartedEventArgs e)
        {
            string prefix = "";
            string suffix = "";
            string project = Path.GetFileNameWithoutExtension(e.ProjectFile);

            switch (e.TargetNames)
            {
                case "Clean":                         return; // No need to show clean messages.
                case "GetCopyToOutputDirectoryItems": return; // No need to show "copying to target" messages.

                case "Rebuild":
                    if (visited.Contains(project))
                        return;
                    visited.Add(project);
                    prefix = "Rebuilding ";
                    break;

                case "Build":
                    if (visited.Contains(project))
                        return;
                    visited.Add(project);
                    prefix = "Building ";
                    break;

                case "GetNativeManifest":
                    prefix = "  Building manifest for ";
                    break;

                case "GetResolvedLinkLibs":
                    prefix = "  Linking ";
                    break;

                case "":
                    if (visited.Contains(project))
                        return;
                    visited.Add(project);
                    break;

                default:
                    suffix = " (" + e.TargetNames + ")";
                    break;
            }

            outputMgr.EndProgress();
            outputMgr.StartProgress(prefix + project + suffix);
        }

        private void OnErrorRaised(object sender, BuildErrorEventArgs e)
        {
            outputMgr.Warning(e.Message);
        }

        private void OnTaskStarted(object sender, TaskStartedEventArgs e)
        {
            outputMgr.Progress();
        }

        private void OnBuildFinished(object sender, BuildFinishedEventArgs e)
        {
            outputMgr.EndProgress();
        }

        public string          Parameters { get { return ""; } set { } }
        public LoggerVerbosity Verbosity { get { return LoggerVerbosity.Normal; } set { } }
        public void Shutdown() { }
    }
}