namespace Mogre.Builder.Tasks
{
    class BuildMogre : MsBuildTask
    {
        public BuildMogre(InputManager inputManager, OutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr) 
        { 
        }

        public override string ID          { get { return "mogre:build"; } }
        public override string Name        { get { return "Building Mogre"; } }
        public override string Description { get { return "Builds Mogre itself."; } }

        public override void Run()
        {
            string solutionFile;
            switch (inputManager.VisualStudio)
            {
                case InputManager.VisualStudioVersion.vs2010:
                    solutionFile = inputManager.MogreSolutionFile_VS2010;
                    break;
                case InputManager.VisualStudioVersion.vs2012:
                    solutionFile = inputManager.MogreSolutionFile_VS2012;
                    break;
                case InputManager.VisualStudioVersion.vs2013:
                    solutionFile = inputManager.MogreSolutionFile_VS2013;
                    break;
                default:
                    throw new System.NotImplementedException("Unknown version of Visual Studio");
            }

            string platform = inputManager.Option_x64 ? "x64" : "Win32";

            msBuildManager.Build(solutionFile, inputManager.BuildConfiguration, platform, "Rebuild");
        }
    }
}
