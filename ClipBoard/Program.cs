using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ClipBoard
{
    static class Program
    {
        private const int HTCAPTION = 0x2;
        private const int WH_KEYBOARD_LL = 13;
        private static Win32Hooks.LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static MainForm mf;
        public static string ContentFileName;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // check if a content file has been provided in command line,
            // otherwise set its name to the users %APDDATA% 
            var commandLineArgs = Environment.GetCommandLineArgs();
            ContentFileName = commandLineArgs.Length > 1 ? 
                commandLineArgs[1] : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Clipboard//content.csv");

            _hookID = SetHook(_proc);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            mf = new MainForm();
            Application.Run(mf);
            Win32Hooks.UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(Win32Hooks.LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return Win32Hooks.SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    Win32Hooks.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)Msgs.WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys keys = (Keys)vkCode;
                mf.keyPressedHandler(keys);
            }
            return Win32Hooks.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
    }
}
