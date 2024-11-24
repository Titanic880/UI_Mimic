using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;
using UI_Mimic.Windows;
using UI_Mimic.Linux;

namespace UI_Mimic {
    public abstract class InputReader : IDisposable {

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
            
        }
        protected InputReader(bool Global, string[] LoggingWindows) {
            this.Global = Global;
            this.LoggingWindows = LoggingWindows;
        }/*
        private static InputReader HookBuilder(HookTypePub typePub, string[] AllowedWindows, bool global=true) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return new W_UIReader(global, AllowedWindows);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                //Currently unsupported.
                throw new NotImplementedException("Linux is not supported currently.");
                //return new L_UIReader(global,AllowedWindows);
            }
            throw new NotImplementedException("OSX is not supported.");
        }*/
        public virtual bool GenerateHook(HookTypePub typePub) {
            /*
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {


            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                //Currently unsupported.
                return false;
            }*/
            return false;
        }
        public virtual bool DisconnectHook(HookTypePub typePub) {
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual byte GetHookState() {
            byte ret = 0;
            if (_keyboardHookId != IntPtr.Zero) {
                ret ++;
            }
            if(_mouseHookId != IntPtr.Zero) {
                ret += 16;
            }
            return ret;
        }

        /// <summary>
        /// Returns a new implementation of the InputReader class based on operating system
        /// </summary>
        /// <param name="Global"></param>
        /// <param name="AllowedWindows"></param>
        /// <returns></returns>
        public static InputReader ReaderFactory(bool Global, string[] AllowedWindows) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return new Windows.W_UIReader(Global, AllowedWindows);
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
#if DEBUG
                return new Linux.L_UIReader(Global, AllowedWindows);
#endif
                throw new NotSupportedException("Linux Support is WIP");
            } else {
                throw new NotSupportedException("Operating system you are running currently does not support this project.");
            }
        }
    }
}
