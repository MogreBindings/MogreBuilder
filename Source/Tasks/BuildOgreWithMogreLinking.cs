using System;

namespace Mogre.Builder.Tasks
{
    class BuildOgreWithMogreLinking : BuildOgreTask
    {
        public BuildOgreWithMogreLinking(InputManager inputManager, OutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr) 
        { 
        }

        public override string ID          { get { return "ogre:build-with-mogre"; } }
        public override string Name        { get { return "Building Ogre with linking back to Mogre"; } }
        public override string Description { get { return "Builds Ogre with linking back to Mogre. This is the final pass that requires Mogre to have been built and generates the final Ogre binaries that can be used with Mogre."; } }

        public override void Run()
        {
            BuildOgre(inputManager.BuildConfiguration, false, true);
        }
    }
}