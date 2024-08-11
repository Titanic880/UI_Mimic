using System.Runtime.InteropServices;
using System.Linq;
using System;

namespace UI_Mimic.Linux {
    internal class L_UIReader : InputReader {
        //Resources:
        //https://stackoverflow.com/questions/6560553/linux-x11-global-keyboard-hook
        //

        [DllImport("input.h",CallingConvention = CallingConvention.StdCall)]
        private static extern void DEBUG();
        public L_UIReader(bool Global, string[] LoggingWindows) :
            base(Global, LoggingWindows) {
        }

        public override bool GenerateHook(HookTypePub PubHook) {
            throw new NotImplementedException();/*
            switch (PubHook) {
            case HookTypePub.Keyboard:
                KeyboardHook = new CallbackDelegate(KeyboardHookProc);
                break;
            case HookTypePub.Mouse:
                MouseHook = new CallbackDelegate(MouseHookProc);
                break;
            }*/
        }
        public override bool DisconnectHook(HookTypePub PubHook) {
            throw new NotImplementedException();/*
            switch (PubHook) {
            case HookTypePub.Keyboard:
                if(KeyboardHook == null) {
                    return true;
                }
                //Unhook Here

                KeyboardHook = null;
                _keyboardHookId = IntPtr.Zero;
                break;
            case HookTypePub.Mouse:
                if(MouseHook == null) {
                    return true;
                }
                //Unhook Here

                MouseHook = null;
                _mouseHookId = IntPtr.Zero;
                break;
            }

            return true;*/
        }

        private bool SafetyChecks() {
            string ActiveWindow = L_WindowInfo.GetActiveWindowTitle();//TODO: GetActiveWindow
            if(ActiveWindow == null) {
                return false;
            }
            if (Global) {   //Inaccurate Check
                return LoggingWindows.Any(x=> ActiveWindow.ToLower().Contains(x.ToLower()));
            } else {        //Accurate Check
                return LoggingWindows.Any(x => x.Contains(ActiveWindow));
            }
        }

        private bool IsFinalized = false;
        ~L_UIReader() {
            if (IsFinalized) {
                return;
            }
            //UnHook Here

            IsFinalized = true;
        }
        public new void Dispose() {
            if (IsFinalized) {
                return;
            }
            //UnHook Here

            IsFinalized = true;
            base.Dispose();
        }

        [MTAThread]
        private int KeyboardHookProc(int code, IntPtr EventPtr, IntPtr InputPtr) {
            if(SafetyChecks() is false) {
                return 0; //TODO: Pass onto next handler (If exists in Linux)
            }
            throw new NotImplementedException();
            
        }
        [MTAThread]
        private int MouseHookProc(int code, IntPtr W, IntPtr L) {
            if (SafetyChecks() is false) {
                return 0; //TODO: Pass onto next handler (If exists in Linux)
            }
            throw new NotImplementedException();
        }
    }

}
