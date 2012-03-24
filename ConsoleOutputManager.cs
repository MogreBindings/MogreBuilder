using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mogre.Builder
{
    public class ConsoleOutputManager : IOutputManager
    {
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
            DisplayMessage(message, ConsoleColor.Red);
        }

        public void Warning(string message)
        {
            DisplayMessage(message, ConsoleColor.Yellow);
        }

        public void Action(string message)
        {
            DisplayMessage(message, ConsoleColor.Cyan);
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
    }
}
