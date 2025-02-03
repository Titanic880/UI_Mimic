using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System;
using System.Windows.Forms;

namespace UI_Mimic.Windows {
    public static class W_WindowInfo {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr WindowHandle);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        
        /// <summary>
        /// Pass in the button you are using for this function (object? sender -> sender)
        /// </summary>
        /// <param name="ControlledButton"></param>
        /// <returns></returns>
        public static string FindWindow(Button ControlledButton) {
            string OGText = ControlledButton.Text;
            MessageBox.Show("After Closing this popup open the window you want to catch the name of\n another popup will happen when it is done...");
            for (int i = 5; i != 0; i--) {
                ControlledButton.Text = i.ToString();
                ControlledButton.Update();
                //Sleep for 1 second so we can update the UI every Second
                System.Threading.Thread.Sleep(1000);
            }
            string selectedwindow = UI_Mimic.Windows.W_WindowInfo.GetActiveWindowTitle();
            DialogResult res = MessageBox.Show($"Window with the name of: '{selectedwindow}' was caught!\n(Please note that names can sometimes be inaccurate to the title provided but still relevant)\n is this correct?","",MessageBoxButtons.YesNo);

            if (res == DialogResult.Yes) {

            }
            ControlledButton.Text = OGText;
            return selectedwindow;
        }
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
