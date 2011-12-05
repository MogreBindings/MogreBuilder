using System;

namespace Mogre.Builder.Tasks
{
    class BuildMogre : MsBuildTask
    {
        public BuildMogre(OutputManager outputMgr, MsBuildManager msBuildMgr) : base(outputMgr, msBuildMgr) { }

        public override string ID          { get { return "mogre:build"; } }
        public override string Name        { get { return "Building Mogre"; } }
        public override string Description { get { return "Builds Mogre itself."; } }

        public override void Run()
        {
            msBuildMgr.Build(@"Main\Mogre_vs2010.sln", "Debug", "Win32", "Rebuild");
        }
    }
}
