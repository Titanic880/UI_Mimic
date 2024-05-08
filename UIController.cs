﻿using System.Windows.Forms;
using System;

namespace UI_Mimic {
    public abstract class UIController: IDisposable {

        protected readonly bool Global = false;
        protected readonly string[] LoggingWindows;

        protected IntPtr _mouseHookId = IntPtr.Zero;
        protected IntPtr _keyboardHookId = IntPtr.Zero;

        protected CallbackDelegate MouseHook = null;
        protected CallbackDelegate KeyboardHook = null;

        protected delegate int CallbackDelegate(int Code, IntPtr W, IntPtr L);

        public delegate void ErrorEventHandler(Exception e);
        public delegate void LocalKeyEventHandler(Keys key, bool Shift, bool Ctrl, bool Alt, bool Home);
        public delegate void LocalMouseMoveHandler(int xPos, int yPos);
        public delegate void LocalMouseEventHandler(MouseButtons MouseAction);
        public delegate void LocalMouseEventUp(MouseButtons MouseAction);
        public delegate void LocalMouseEventDown(MouseButtons MouseAction);

        public event LocalKeyEventHandler KeyDown;
        public event LocalKeyEventHandler KeyUp;
        public event ErrorEventHandler OnError;
        public event LocalMouseMoveHandler OnMouseMove;
        public event LocalMouseEventHandler OnMouseClick;
        public event LocalMouseEventDown OnMouseDown;
        public event LocalMouseEventUp OnMouseUp;
        protected void TriggerKeyDown(Keys Key, bool Shift, bool Ctrl, bool Alt, bool Home) => KeyDown?.Invoke(Key, Shift, Ctrl, Alt, Home);
        protected void TriggerKeyUp(Keys Key, bool Shift, bool Ctrl, bool Alt, bool Home) => KeyUp?.Invoke(Key, Shift, Ctrl, Alt, Home);
        protected void TriggerOnError(Exception e) => OnError?.Invoke(e);
        protected void TriggerOnMouseMove(int xPos, int yPos) => OnMouseMove?.Invoke(xPos, yPos);
        protected void TriggerOnMouseClick(MouseButtons MouseAction) => OnMouseClick?.Invoke(MouseAction);
        protected void TriggerOnMouseDown(MouseButtons MouseAction) => OnMouseDown?.Invoke(MouseAction);
        protected void TriggerOnMouseUp(MouseButtons MouseAction) => OnMouseUp?.Invoke(MouseAction);
        

        public void Dispose() {
            throw new NotImplementedException();
        }

        protected UIController(bool Global, string[] LoggingWindows) {
            this.Global = Global;
            this.LoggingWindows = LoggingWindows;
        }
    }
}
