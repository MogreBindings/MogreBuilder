using System;

namespace Mogre.Builder
{
    class OutputManager
    {
        public void DisplayMessage(string message, ConsoleColor color)
        {
            Console.ResetColor();
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ResetColor();
            Console.WriteLine("");
        }

        public void Warn(string message)
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

        public void Progress()
        {
            Console.Write(".");
        }

        public void EndProgress()
        {
            Console.WriteLine();
        }
    }
}