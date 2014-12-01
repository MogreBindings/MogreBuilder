namespace Mogre.Builder.Tasks
{
    class BuildMoisTask : MsBuildTask
    {
        public BuildMoisTask(InputManager inputManager, OutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr)
        {
        }

        public override string ID { get { return "mois:build"; } }
        public override string Name { get { return "Building MOIS"; } }
        public override string Description { get { return "Builds MOIS addon."; } }

        public override void Run()
        {
            string solutionFile;
            switch (inputManager.VisualStudio)
            {
                case InputManager.VisualStudioVersion.vs2010:
                    solutionFile = inputManager.MoisSolutionFile_VS2010;
                    break;
                case InputManager.VisualStudioVersion.vs2012:
                    solutionFile = inputManager.MoisSolutionFile_VS2012;
                    break;
                case InputManager.VisualStudioVersion.vs2013:
                    solutionFile = inputManager.MoisSolutionFile_VS2012;  // TODO: Check if this solution file works fine (possibly a solution file with TargetFramework 4.5.1 is needed)
                    break;
                default:
                    throw new System.NotImplementedException("Unknown version of Visual Studio");
            }
            
            string platform = inputManager.Option_x64 ? "x64" : "Win32";

            msBuildManager.Build(solutionFile, inputManager.BuildConfiguration, platform, "MOIS");
        }
    }
}
