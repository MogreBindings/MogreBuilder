using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Mogre.Builder.Tasks
{
    internal class NewtonCompilation : Task
    {
        public override string ID
        {
            get { return "newton:Compilation"; }
        }

        public override string Name
        {
            get { return "Compilation of MogreNewt"; }
        }

        public override string Description
        {
            get { return "This task tries to compile MogreNewt."; }
        }

        public NewtonCompilation(InputManager inputManager, OutputManager outputManager)
            : base(inputManager, outputManager)
        {
        }

        public override void Run()
        {

            // TODO
            outputManager.Warning("The compilation isn't implemented yet !!");
            throw new NotImplementedException("Application stopped");


            // print success message
            outputManager.AddBlankLine();
            outputManager.DisplayMessage("The MogreNewt build process seems to be finished successfully (-:", ConsoleColor.Green);

        }

    } //  class NewtonCompilation
}
