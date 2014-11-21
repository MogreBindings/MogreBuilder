using System;
using System.Collections.Generic;
using System.IO;

namespace Mogre.Builder
{
    public class OutputManager
    {
        // COLOURS
        private ConsoleColor errorColour = ConsoleColor.Red;
        private ConsoleColor warningColour = ConsoleColor.Yellow;
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

        /// <summary>Class to create a HTML logfile.</summary>
        private HtmlLogger htmlLog;



        /// <summary>
        /// Output a message to the console with user defined colour.
        /// </summary>
        /// <param name="message">Message to print</param>
        /// <param name="color">Colour enum</param>
        public void DisplayMessage(string message, ConsoleColor color)
        {
            // move to new line if needed
            if (Console.CursorLeft > 0)
                AddBlankLine();

            Console.ResetColor();
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
            Console.WriteLine("");

            htmlLog.WriteLine(message, color);
        }




        /// <summary>
        /// Add a blank line for better overview. 
        /// If the line buffer contains text, it will be flushed before the blank line. 
        /// </summary>
        public void AddBlankLine()
        {
            Console.WriteLine();
            htmlLog.WriteBlankLine();
        }



        /// <summary>
        /// Output an error to the console in red colour.
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Error(string message)
        {
            DisplayMessage(message, errorColour);

            if (IsErrorRecordingEnabled)
                errorList.Add(new ErrorEntry(currentAction, message, ErrorType.Error));
        }


        /// <summary>
        /// Output a warning to the console in yellow colour.
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Warning(string message)
        {
            DisplayMessage(message, warningColour);

            if(IsErrorRecordingEnabled)
                errorList.Add(new ErrorEntry(currentAction, message, ErrorType.Warning));
        }



        /// <summary>
        /// Output an action message to the console in cyan colour.
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Action(string message)
        {
            AddBlankLine();
            DisplayMessage(message, actionColour);
            currentAction = message;
        }



        /// <summary>
        /// Output a message to the console in default colour (gray).
        /// </summary>
        /// <param name="message">Message to print</param>
        public void Info(string message)
        {
            Console.WriteLine(message);
            htmlLog.WriteLine(message, ConsoleColor.Gray);
        }



        public void StartProgress(string message)
        {
            Console.Write(message);
            htmlLog.Write(message);
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
                htmlLog.Write(".");
            }
        }



        public void EndProgress()
        {
            AddBlankLine();
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


        public string FeatureSummary { get; set; }

        public bool IsFeatureLoggingEnabled { get; set; }

        public string MogreVersion { get; set; }

        public Boolean SuccessfulOgreBuild { get; set; }

        /// <summary>
        /// Can be used to temporarily disable error recording so errors don' show up on PrintSummary()
        /// </summary>
        public bool IsErrorRecordingEnabled { get; set; }

        
        // constructor
        public OutputManager()
        {
            FeatureSummary = "";
            MogreVersion = "??";


            //String fileName = Path.Combine(inputManager.TargetDirectory, inputManager.BuildOutputDirectory)
            String fileName = String.Format("MogreBuilder_logfile.htm");
            htmlLog = new HtmlLogger(fileName, DateTime.Now.Ticks, Environment.CommandLine);
        }



        public void PrintSummary(InputManager inputManager)
        {

            if (FeatureSummary.Length > 0)
            {
                AddBlankLine();
                AddBlankLine();
                DisplayMessage("===================== Feature summary =====================\n", ConsoleColor.White);

                Info(FeatureSummary); // grabbed CMake summary
            }


            if (errorList.Count > 0)
            {
                AddBlankLine();
                AddBlankLine();
                DisplayMessage("===================== Repeat of all errors and warnings =====================\n", ConsoleColor.White);

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

                        case ErrorType.Warning:
                            DisplayMessage(entry.message, warningColour);
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




        public void CloseLogfile()
        {
            htmlLog.CloseLogfile();
        }



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
            Error, Warning
        }

    } //  class OutputManager
}
