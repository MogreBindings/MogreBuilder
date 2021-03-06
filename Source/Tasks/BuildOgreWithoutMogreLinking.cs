﻿using System;

namespace Mogre.Builder.Tasks
{
    class BuildOgreWithoutMogreLinking : BuildOgreTask
    {
        public BuildOgreWithoutMogreLinking(InputManager inputManager, OutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr) 
        { 
        }

        public override string ID          { get { return "ogre:build-without-mogre"; } }
        public override string Name        { get { return "Building Ogre without linking back to Mogre"; } }
        public override string Description { get { return "Builds Ogre without linking the build back to the Mogre binaries. This is the initial pass required in order to build Mogre itself and can be run before Mogre is built."; } }

        public override void Run()
        {
            BuildOgre(inputManager.BuildConfiguration, true, false);
        }
    }
}