using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System;

namespace UI_Mimic.Windows {
    public static class WindowInfo {
        //[DllImport("user32.dll")]
        //private static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        //private const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr WindowHandle);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static void FocusProcess(string processname, bool AccurateCheck = false) {
            void Logic(Process window) {
                //ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
                IntPtr hWnd = window.MainWindowHandle;
                SetForegroundWindow(hWnd);
            }

            Process[] allitems = Process.GetProcesses();
            foreach (Process a in allitems) {
                if (AccurateCheck) {
                    if (a.ProcessName.Equals(processname)) {
                        Logic(a);
                    } else if (a.ProcessName.ToLower().Contains(processname.ToLower())) {
                        Logic(a);
                    }
                }
            }
        }

        public static string GetActiveWindowTitle() {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0) {
                return Buff.ToString();
            }
            return null;
        }
    }
}
