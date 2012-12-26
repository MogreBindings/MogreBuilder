namespace Mogre.Builder.Tasks
{
    class BuildMogre : MsBuildTask
    {
        public BuildMogre(InputManager inputManager, IOutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr) 
        { 
        }

        public override string ID          { get { return "mogre:build"; } }
        public override string Name        { get { return "Building Mogre"; } }
        public override string Description { get { return "Builds Mogre itself."; } }

        public override void Run()
        {
            string solutionFile = inputManager.Option_Vs2012 ? inputManager.MogreSolutionFile_VS2012 : inputManager.MogreSolutionFile_VS2010;
            string platform = inputManager.Option_x64 ? "x64" : "Win32";

            msBuildManager.Build(solutionFile, inputManager.BuildConfiguration, platform, "Rebuild");
        }
    }
}
