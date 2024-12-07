using System.Runtime.InteropServices;
using System.Windows.Forms;
using System;
using System.Linq;

namespace UI_Mimic {
    public abstract class InputReader : IDisposable {

        protected readonly bool Global = false;
        protected readonly string[] LoggingWindows;
        //These are exclusively 1 of 2 objects, Extensive checks need to be done prior to using them
        //(Switch these to struct of MouseAction & Keys ?)
        protected Debug_Feature_01_MultiTypeStorage Debug_Feature_01_Replacement_Target;   //Item to look for to replace
        protected Debug_Feature_01_MultiTypeStorage Debug_Feature_01_Replacement_Replace;  //new item to replace it
        public bool Debug_Feature_01_LockReplacement { get; protected set; } = false;


        protected IntPtr _mouseHookId = IntPtr.Zero;
        protected IntPtr _keyboardHookId = IntPtr.Zero;
        protected IntPtr[] _Debug_Feature_01_Replacement_IntPtr = new IntPtr[2];

        protected CallbackDelegate MouseHook = null;
        protected CallbackDelegate KeyboardHook = null;
        protected CallbackDelegate Debug_Feature_01_Replacement_CBD = null;

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
        public static InputReader GetInputReader(bool Global, string[] AllowedWindows) {
            if(AllowedWindows.Length <= 0) {
                throw new ArgumentException("AllowedWindows at/below 0 index size");
            } else if (AllowedWindows.Contains("")) {
                throw new ArgumentException("AllowedWindows cannot contain blank indexes");
            }

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
        [Obsolete("Method Name has been changed to: GetInputReader")]
        public static InputReader ReaderFactory(bool Global, string[] AllowedWindows) {
            return GetInputReader(Global, AllowedWindows);
        }
    }
}
