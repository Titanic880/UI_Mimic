using UI_Mimic.Windows;

namespace UI_Mimic_Testing {
    [TestClass]
    public class Test_UIReader {
        internal void TestState(Func<bool> KeyFunc, ushort Key) {
            Assert.IsFalse(KeyFunc());
            UI_Mimic.Windows.UserControl.InputHold(Key);
            Assert.IsTrue(KeyFunc());
            UI_Mimic.Windows.UserControl.InputRelease();
            Assert.IsFalse(KeyFunc());
        }

        [TestMethod]
        public void TestKeyStateCapsLock() {
            //ushort CapsLockCode = (ushort)System.Windows.Forms.Keys.CapsLock;
            TestState(UIReader.GetCapslock, 0x000);
        }
        [TestMethod]
        public void TestKeyStateNumLock() {
            TestState(UIReader.GetNumlock, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateScrollLock() {
            TestState(UIReader.GetScrollLock, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateHome() {
            TestState(UIReader.GetHomePressed, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateShift() {
            TestState(UIReader.GetShiftPressed, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateCtrl() {
            TestState(UIReader.GetCtrlPressed,0x0000);
        }
        [TestMethod]
        public void TestKeyStateAlt() {
            TestState(UIReader.GetAltPressed,0x0000);
        }
    }
}
