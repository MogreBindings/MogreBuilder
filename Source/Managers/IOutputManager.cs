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
        void DisplayMessage(string message, ConsoleColor color);

        String FeatureSummary { get; set; }
        Boolean FeatureLoggingIsEnabled { get; set; }
    }
}