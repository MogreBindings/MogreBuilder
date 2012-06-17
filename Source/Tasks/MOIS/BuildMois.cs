using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            msBuildManager.Build(inputManager.MoisSolutionFile, inputManager.BuildConfiguration, "Win32", "Rebuild");
        }
    }
}
