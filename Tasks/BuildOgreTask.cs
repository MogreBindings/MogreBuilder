using System;

namespace Mogre.Builder.Tasks
{
    abstract class BuildOgreTask : MsBuildTask
    {
        public BuildOgreTask(OutputManager outputMgr, MsBuildManager msBuildMgr) : base(outputMgr, msBuildMgr) { }
        
        protected void BuildOgre(string configuration, bool rebuild, bool linkToMogre)
        {
            ModifyFile(@"Main\OgreSrc\ogre\OgreMain\include\CLRConfig.h", @"LINK_TO_MOGRE\s+\d+", "LINK_TO_MOGRE " + (linkToMogre ? "1" : "0"));

            var targetSuffix = rebuild ? ":Rebuild" : "";
            msBuildMgr.Build(
                @"Main\OgreSrc\build\OGRE.sln", configuration, "Win32",
                new string[] {
                    "OgreMain"                  + targetSuffix,
                    "OgrePaging"                + targetSuffix,
                    "OgreRTShaderSystem"        + targetSuffix,
                    "OgreTerrain"               + targetSuffix,
                    "Plugin_BSPSceneManager"    + targetSuffix,
                    "Plugin_CgProgramManager"   + targetSuffix,
                    "Plugin_OctreeSceneManager" + targetSuffix,
                    "Plugin_OctreeZone"         + targetSuffix,
                    "Plugin_ParticleFX"         + targetSuffix,
                    "Plugin_PCZSceneManager"    + targetSuffix,
                    "RenderSystem_Direct3D9"    + targetSuffix,
                    "RenderSystem_GL"           + targetSuffix,
                }
            );
        }
    }
}