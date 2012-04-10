using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
//using System.Drawing;
using System.Text;
//using System.Windows.Forms;


namespace Mogre.Builder
{
    public class TaskbarHighlight
    {
        // THIS CODE IS FROM
        // http://www.xnamag.de/forum/viewtopic.php?t=2010

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        extern static int SetForegroundWindow(IntPtr HWnd);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        extern static IntPtr GetForegroundWindow();


        /// <summary>
        /// Check if the current window is in foreground. 
        /// </summary>
        public static Boolean WindowIsInForeground()
        {
            IntPtr currentWindow = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            IntPtr foregroundWindow = GetForegroundWindow();

            if (currentWindow == foregroundWindow)
                return true;
            else
                return false;
        }




        struct FLASHWINFO
        {
            public Int32 cbSize;
            public IntPtr hwnd;
            public Int32 dwFlags;
            public Int32 uCount;
            public Int32 twTimeout;
        }

        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int FlashWindowEx(ref FLASHWINFO pwfi);


        /// <summary>
        /// Blink in taskbar until the window gets the focus.
        /// </summary>
        public static void BlinkUntilFocus()
        {
            FLASHWINFO flash = new FLASHWINFO();
            flash.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(flash);
            flash.hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            flash.dwFlags = Flags.FLASHW_ALL | Flags.FLASHW_TIMERNOFG;
            flash.uCount = Int32.MaxValue;
            flash.twTimeout = 5000;
            FlashWindowEx(ref flash);
        }



        /// <summary>
        /// Blink in taskbar for a few seconds. (Even when the windows has the focus.)
        /// </summary>
        public static void BlinkShortly()
        {
            FLASHWINFO flash = new FLASHWINFO();
            flash.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(flash);
            flash.hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            flash.dwFlags = Flags.FLASHW_ALL;
            flash.uCount = 1;
            flash.twTimeout = 1000;
            FlashWindowEx(ref flash);
        }



        /// <summary>
        /// Highlight forever the taskbar without blinking.
        /// </summary>
        public static void StaticHighlight()
        {
            FLASHWINFO flash = new FLASHWINFO();
            flash.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(flash);
            flash.hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

            flash.dwFlags = Flags.FLASHW_TRAY;
            flash.uCount = Int32.MaxValue;
            flash.twTimeout = 1;
            FlashWindowEx(ref flash);
        }



        class Flags
        {
            /// <summary>Stop flashing. The system restores the window to its original state. </summary>
            public const Int32 FLASHW_STOP = 0;

            /// <summary>Flash the window caption. </summary>
            public const Int32 FLASHW_CAPTION = 1;

            /// <summary>Flash the taskbar button. </summary>
            public const Int32 FLASHW_TRAY = 2;

            /// <summary>Flash both the window caption and taskbar button.
            ///          This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. </summary>
            public const Int32 FLASHW_ALL = 3;

            /// <summary>Flash continuously, until the FLASHW_STOP flag is set. </summary>
            public const Int32 FLASHW_TIMER = 4;

            /// <summary>Flash continuously until the window comes to the foreground. </summary>
            public const Int32 FLASHW_TIMERNOFG = 12;
        } // class FlashFlags




    } // class TaskbarHighlight
} // namespace 