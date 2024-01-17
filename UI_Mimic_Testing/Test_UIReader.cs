using UI_Mimic;

namespace UI_Mimic_Testing {
    [TestClass]
    public class Test_UIReader {
        internal void TestState(Func<bool> KeyFunc, ushort Key) {
            Assert.IsFalse(KeyFunc());
            UI_Mimic.UserControl.InputHold(Key);
            Assert.IsTrue(KeyFunc());
            UI_Mimic.UserControl.InputRelease();
            Assert.IsFalse(KeyFunc());
        }

        [TestMethod]
        public void TestKeyStateCapsLock() {
            //ushort CapsLockCode = (ushort)System.Windows.Forms.Keys.CapsLock;
            TestState(UI_Mimic.UIReader.GetCapslock, 0x000);
        }
        [TestMethod]
        public void TestKeyStateNumLock() {
            TestState(UI_Mimic.UIReader.GetNumlock, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateScrollLock() {
            TestState(UI_Mimic.UIReader.GetScrollLock, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateHome() {
            TestState(UI_Mimic.UIReader.GetHomePressed, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateShift() {
            TestState(UI_Mimic.UIReader.GetShiftPressed, 0x0000);
        }
        [TestMethod]
        public void TestKeyStateCtrl() {
            TestState(UI_Mimic.UIReader.GetCtrlPressed,0x0000);
        }
        [TestMethod]
        public void TestKeyStateAlt() {
            TestState(UI_Mimic.UIReader.GetAltPressed,0x0000);
        }
    }
}
