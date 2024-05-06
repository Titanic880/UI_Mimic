using System.Windows.Forms;
using System;

namespace UI_Mimic {
    public abstract class UIController: IDisposable {

        protected readonly bool Global = false;
        protected readonly string[] LoggingWindows;

        public delegate void ErrorEventHandler(Exception e);
        public delegate void LocalKeyEventHandler(Keys key, bool Shift, bool Ctrl, bool Alt, bool Home);
        public delegate void LocalMouseMoveHandler(int xPos, int yPos);
        public delegate void LocalMouseEventHandler(MouseButtons MouseAction);
        public delegate void LocalMouseEventUp(MouseButtons MouseAction);
        public delegate void LocalMouseEventDown(MouseButtons MouseAction);        

        public void Dispose() {
            throw new NotImplementedException();
        }

        protected UIController(bool Global, string[] LoggingWindows) {
            this.Global = Global;
            this.LoggingWindows = LoggingWindows;
        }
    }
}
