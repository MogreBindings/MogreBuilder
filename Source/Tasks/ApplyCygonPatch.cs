using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Mogre.Builder
{
    /// <summary>
    /// This patch adds some #pragma managed(push, off) sections around unmanaged
    /// headers, preventing the compiler from generating MSIL representations of
    /// countless C++ standard library types that aren't used on the C# side anyway.
    /// 
    /// The size of the resulting binaries will be reduced. e.g.
    /// Mogre.dll (x86) from 12.3 MiB to 8.1 MiB
    /// Mogre.dll (x64) from 20.8 MiB to 9.0 MiB
    /// 
    /// It also moves the shutdown code from the finalizer of Mogre.Root into its
    /// destructor (IDisposable.Dispose()). This is done because Ogre's OpenGL driver
    /// couldn't cope with the shutdown happening from the GC thread. Maybe future
    /// versions of Ogre won't need this change.
    /// </summary>
    class ApplyCygonPatch : Task
    {
        public ApplyCygonPatch(InputManager inputManager, OutputManager outputManager)
            : base(inputManager, outputManager) 
        { 
        }

        public override string ID          { get { return "cygon:patch"; } }
        public override string Name        { get { return "Applying Cygon patch"; } }
        public override string Description { get { return "Applies an extra patch prevent the compiler from generating MSIL representations " +
            "of countless C++ standard library types that aren't used on the C# side anyway."; } }

        public override void Run()
        {
            String patchExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "patch.exe");
            String patchFilePath = Path.Combine(Directory.GetCurrentDirectory(), inputManager.CygonPatchFile);
            CommandResult result = RunCommand(patchExe, String.Format("-p0 -i \"{0}\"", patchFilePath), inputManager.TargetDirectory);

            if (result.ExitCode != 0)
            {
                // Files are cleaned during cloning/updating. If patching failed, something went wrong.

                // prepare message
                String message = "Cygon patch failed";
                if (result.Error != "")
                    message += ":  " + result.Error;

                throw new Exception(message);
            }

        } // Run()


    } // class ApplyCygonPatch
} // namespace
