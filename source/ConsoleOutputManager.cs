using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogre.Builder
{
    public class ConsoleOutputManager : IOutputManager
    {
        // COLOURS
        private ConsoleColor errorColour = ConsoleColor.Red;
        private ConsoleColor waringColour = ConsoleColor.Yellow;
        private ConsoleColor actionColour = ConsoleColor.Cyan;

        // list of warnings and errors
        private List<ErrorEntry> errorList = new List<ErrorEntry>();
        private String currentAction = "";

        public void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ResetColor();
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
            Console.WriteLine("");
        }

        public void Error(string message)
        {
            DisplayMessage(message, errorColour); // RED
            errorList.Add(new ErrorEntry(currentAction, message, ErrorType.Error));
        }

        public void Warning(string message) // YELLOW
        {
            DisplayMessage(message, waringColour);
            errorList.Add(new ErrorEntry(currentAction, message, ErrorType.Waring));
        }

        public void Action(string message) // CYAN
        {
            DisplayMessage(message, actionColour);
            currentAction = message;
        }

        public void Info(string message)
        {
            Console.WriteLine(message);
        }

        public void StartProgress(string message)
        {
            Console.Write(message);
        }

        private int animationIndex = 0;
        private char[] animationChars = { '-', '\\', '|', '/' };
        
        public void Progress()
        {
            if (Console.CursorLeft == 0)
            {
                Console.CursorVisible = false;
                Console.Write(animationChars[animationIndex]);
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                animationIndex++;

                if (animationIndex == animationChars.Length)
                    animationIndex = 0;
            }
            else
            {
                Console.Write(".");
            }
        }

        public void EndProgress()
        {
            Console.WriteLine();
            Console.CursorVisible = true;
        }



        public void PrintSummary()
        {
            DisplayMessage("\n\n===================== Repeat of all errors and warning =====================\n", ConsoleColor.White);

            String lastAction = "";

            foreach (ErrorEntry entry in errorList)
            {
                // print each action only once
                if (entry.action != lastAction)
                {
                    DisplayMessage(entry.action, actionColour);
                    lastAction = entry.action;
                }

                switch (entry.type)
                {
                    case ErrorType.Error:
                        DisplayMessage(entry.message, errorColour);
                        break;

                    case ErrorType.Waring:
                        DisplayMessage(entry.message, waringColour);
                        break;

                    default:
                        throw new NotImplementedException("Add new ErrorType to switch");
                }

            } // foreach

            //Console.WriteLine("\nIf you need more details, scroll through all messages." + 
            //                  "If you don't see the whole history, set the console 'height buffer' to 9999." + 
            //                  "For this rickt-click to the title of the console window, click to 'Preferences' and choose tab 'Layout'. \n");

            // TODO:  Print the Ogre build settings here  (They are displayed somewhen at the build process. --> catch the message )

        } // PrintSummary()




        /// <summary>
        /// Contains a log entry for and error or warning
        /// </summary>
        private class ErrorEntry
        {
            public String action;
            public ErrorType type;
            public String message;

            public ErrorEntry(String section, String message, ErrorType type)
            {
                this.action = section;
                this.message = message;
                this.type = type;
            }
        } // class ErrorEntry



        private enum ErrorType
        {
            Error, Waring
        }

    } //  class ConsoleOutputManager
}
