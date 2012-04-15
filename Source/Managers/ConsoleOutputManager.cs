using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

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

        /// <summary>If a worker process prints a message with a keyword (phrase), it will be highlighted as warning.</summary>
        private static String[] warningKeywords = 
        {
            "Could not locate", 
            "Could NOT find", 
            "could NOT be located", 
            "does not exist"
        };


        public void DisplayMessage(string message, ConsoleColor color)
        {
            // move to new line if needed
            if (Console.CursorLeft > 0)
                Console.WriteLine();

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

            // debugging
            if (
                Regex.IsMatch(message, @"boost-date_time") ||
                Regex.IsMatch(message, @"Notice: Output directory .* does not exist. I have created it for you.") ||
                Regex.IsMatch(message, @"Generating XML output for file OgreTextureManager.h") ||
                Regex.IsMatch(message, @"send_error")
                
                )
            {
                // Debug breakpoint
            }
        }



        public void Warning(string message) // YELLOW
        {
            DisplayMessage(message, waringColour);
            errorList.Add(new ErrorEntry(currentAction, message, ErrorType.Waring));
        }



        public void Action(string message) // CYAN
        {
            Console.WriteLine();
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


        /// <summary>
        /// Checks if a message of a worker process contains a keyword (phrase), that should be highlighted as warning.
        /// </summary>
        public static Boolean ContainsWarningKeyword(String message)
        {
            foreach (String keyword in warningKeywords)
                if (message.Contains(keyword))
                    return true;

            return false;
        }



        public String FeatureSummary 
        {
            get { return featureSummary; }
            set { featureSummary = value; }
        }
        private String featureSummary = "";


        public Boolean FeatureLoggingIsEnabled 
        {
            get { return featureLoggingIsEnabled; }
            set { featureLoggingIsEnabled = value; }
        }
        private Boolean featureLoggingIsEnabled = false;


        public String MogreVersion 
        {
            get { return mogreVersion; }
            set { mogreVersion = value; } 
        }
        private String mogreVersion = "??";

        public Boolean SuccessfulOgreBuild { get; set; }




        public void PrintSummary(InputManager inputManager)
        {

            if (FeatureSummary.Length > 0)
            {
                DisplayMessage("\n\n===================== Feature summary =====================\n", ConsoleColor.White);

                Info(FeatureSummary); // grabbed CMake summary
            }


            if (errorList.Count > 0)
            {
                DisplayMessage("\n\n===================== Repeat of all errors and warnings =====================\n", ConsoleColor.White);

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
            }


            // success message
            if (SuccessfulOgreBuild)
            {
                DisplayMessage("\n\n===================== Result =====================\n", ConsoleColor.White);

                Info("Mogre version:        " + MogreVersion);
                Info("Build configuration:  " + inputManager.BuildConfiguration + "\n");
                DisplayMessage("The Ogre/Mogre build process seems to be finished successfully (-: \n", ConsoleColor.Green);
                Info("You find the created binary files in: \n    " 
                    + Path.Combine(inputManager.TargetDirectory, inputManager.BuildOutputDirectory));
            }


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
