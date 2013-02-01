using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Mogre.Builder
{
    /// <summary>
    /// This class observes the operation system and adjust the priority of helper processes, which causes much CPU load (e.g. by compiling).
    /// </summary>
    class ProcessPriorityControl
    {
        private static Boolean printPriorityMessages = false;  // if true, messages for many processes will be printed



        // name of extensive worker threads which to control
        static String[] threadNames = { "cmake", "cl", "doxygen", "link", "mspdbsrv" };
        //                              NOTE: Process "hg" is not in list, because it causes only low CPU load

        public static void StartController(ProcessPriorityClass priority, OutputManager outputManager)
        {
            ThreadParams threadParams = new ThreadParams(priority, outputManager);

            Thread controllerThread = new Thread(new ParameterizedThreadStart(ControlThread.DoWork));
            controllerThread.IsBackground = true; // stop thread when main thread stops
            controllerThread.Start(threadParams);

            outputManager.Info("Priority for worker processes:  " + priority.ToString());
        }




        /// <summary>
        /// The instance of this class runs as own tread and observes the system for specific processes. 
        /// </summary>
        class ControlThread
        {

            public static void DoWork(object data)
            {
                ThreadParams p = (ThreadParams)data;

                while (true)
                {
                    foreach (String name in threadNames)
                        foreach (Process process in Process.GetProcessesByName(name))
                        {
                            try
                            {
                                if (process.PriorityClass != p.priority)
                                {
                                    process.PriorityClass = p.priority;

                                    if (printPriorityMessages == true)
                                    {
                                        // create message
                                        String message = String.Format("Changed process priority:  {0}.exe -> {1}", name, p.priority);

                                        // move to new line if needed
                                        if (Console.CursorLeft > 0)
                                            message = "\n" + message;

                                        // print
                                        p.outputManager.DisplayMessage(message, ConsoleColor.White);

                                        // TODO: Maybe add LOCK for call of outputManager.DisplayMessage()
                                        //       Because more than 1 thread call it.
                                    }
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                // happens when the process just ended
                            }
                        }
                    Thread.Sleep(1000);
                }
            } // DoWork()

        } // class ControlThread



        public struct ThreadParams
        {
            public ProcessPriorityClass priority;
            public OutputManager outputManager;

            public ThreadParams(ProcessPriorityClass priority, OutputManager outputManager)
            {
                this.priority = priority;
                this.outputManager = outputManager;
            }
        }

    } // class ProcessPriorityControl
} // namespace
