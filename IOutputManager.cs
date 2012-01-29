using System;

namespace Mogre.Builder
{
    public interface IOutputManager
    {
        void Error(string message);
        void Warning(string message);
        void Action(string message);
        void Info(string message);
        void StartProgress(string message);
        void Progress();
        void EndProgress();
    }
}