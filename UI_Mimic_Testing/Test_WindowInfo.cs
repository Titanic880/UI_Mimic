using System.Diagnostics;

namespace UI_Mimic_Testing {
    [TestClass]
    public class Test_WindowInfo {
        [TestMethod]
        public void WindowFound() {
            string activeWindow = UI_Mimic.WindowInfo.GetActiveWindowTitle();
            Assert.IsNotNull(activeWindow);
            Assert.AreNotEqual(activeWindow, "");
        }
        [TestMethod]
        public void ProcessFocusTest() {
            string FocusAppexe1 = "notepad.exe";
            string FocusAppexe2 = "calc.exe";
            string FocusAppName1 = "Untitled - Notepad";
            string FocusAppName2 = "Calculator";

            Process app1 = Process.Start(FocusAppexe1);
            Process app2 = Process.Start(FocusAppexe2);
            try {

                //Check that the apps are different
                //check that newest app has focus
                //and then try switching App focus 4 times with name
                Assert.IsNotNull(app1);
                Assert.IsNotNull(app2);

                Assert.AreNotSame(FocusAppexe1, FocusAppexe2);
                string Debug_01 = UI_Mimic.WindowInfo.GetActiveWindowTitle();
                Assert.AreEqual(Debug_01, FocusAppName2);

                UI_Mimic.WindowInfo.FocusProcess(FocusAppName1,true);
                string Debug_02 = UI_Mimic.WindowInfo.GetActiveWindowTitle();
                Assert.AreEqual(Debug_02, FocusAppName1);

                UI_Mimic.WindowInfo.FocusProcess(FocusAppName2, true);
                string Debug_03 = UI_Mimic.WindowInfo.GetActiveWindowTitle();
                Assert.AreEqual(Debug_03, FocusAppName2);

                UI_Mimic.WindowInfo.FocusProcess(FocusAppName1, true);
                string Debug_04 = UI_Mimic.WindowInfo.GetActiveWindowTitle();
                Assert.AreEqual(Debug_04, FocusAppName1);

                UI_Mimic.WindowInfo.FocusProcess(FocusAppName2, true);
                string Debug_05 = UI_Mimic.WindowInfo.GetActiveWindowTitle();
                Assert.AreEqual(Debug_05, FocusAppName2);
            } catch { } finally {
                //Close the newest 2 of the processes as cleanup
                app1.Kill();
                app2.Kill();
            }
        }
    }
}