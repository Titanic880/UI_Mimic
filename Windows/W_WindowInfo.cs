using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System;

namespace UI_Mimic.Windows {
    public static class W_WindowInfo {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr WindowHandle);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public static void FocusProcess(string processname, bool AccurateCheck = false) {
            void Logic(Process window) {
                IntPtr hWnd = window.MainWindowHandle;
                SetForegroundWindow(hWnd);
            }

            //Cannot do inaccurate check with this, so we use below method instead.
            //Process[] processes = Process.GetProcessesByName(processname);

            Process[] allitems = Process.GetProcesses();
            
            foreach (Process a in allitems) {
                if (AccurateCheck) {
                    if (a.ProcessName.Equals(processname)) {
                        Logic(a);
                        return;
                    } else if (a.ProcessName.ToLower().Contains(processname.ToLower())) {
                        Logic(a);
                        return;
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
