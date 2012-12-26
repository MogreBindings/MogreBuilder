namespace Mogre.Builder.Tasks
{
    class BuildMoisTask : MsBuildTask
    {
        public BuildMoisTask(InputManager inputManager, IOutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr)
        {
        }

        public override string ID { get { return "mois:build"; } }
        public override string Name { get { return "Building MOIS"; } }
        public override string Description { get { return "Builds MOIS addon."; } }

        public override void Run()
        {
            string solutionFile = inputManager.Option_Vs2012 ? inputManager.MoisSolutionFile_VS2012 : inputManager.MoisSolutionFile_VS2010;
            string platform = inputManager.Option_x64 ? "x64" : "Win32";

            msBuildManager.Build(solutionFile, inputManager.BuildConfiguration, platform, "Rebuild");
        }
    }
}
