using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System;

namespace UI_Mimic
{
    public static class WindowInfo
    {
        //[DllImport("user32.dll")]
        //private static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        //private const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr WindowHandle);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static void FocusProcess(string processname)
        {
            Process[] allitems = Process.GetProcesses();
            foreach (Process a in allitems)
                if (a.ProcessName.ToLower().Contains(processname.ToLower()))
                {
                    IntPtr hWnd = a.MainWindowHandle;
                    //ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
                    SetForegroundWindow(hWnd);
                }
        }

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
    }
}
