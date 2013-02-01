using System;

namespace Mogre.Builder.Tasks
{
    abstract class BuildOgreTask : MsBuildTask
    {
        public BuildOgreTask(InputManager inputManager, OutputManager outputManager, MsBuildManager msBuildMgr)
            : base(inputManager, outputManager, msBuildMgr) 
        { 
        }
        
        protected void BuildOgre(string configuration, bool rebuild, bool linkToMogre)
        {
            ModifyFile(inputManager.ClrConfigHeaderFile, @"LINK_TO_MOGRE\s+\d+", "LINK_TO_MOGRE " + (linkToMogre ? "1" : "0"));

            string platform = inputManager.Option_x64 ? "x64" : "Win32";

            String targetSuffix = rebuild ? ":Rebuild" : "";
            msBuildManager.Build(
                inputManager.OgreSolutionFile, configuration, platform,
                new string[] 
                {
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