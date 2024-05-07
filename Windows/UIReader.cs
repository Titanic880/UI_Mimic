using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System;

namespace UI_Mimic.Windows {
    /// <summary>
    /// Taken and modified for use from this post.
    /// https://stackoverflow.com/questions/34281223/c-sharp-hook-global-keyboard-events-net-4-0
    /// </summary>
    public class UIReader : UIController {
        private IntPtr _mouseHookId = IntPtr.Zero;
        private IntPtr _keyboardHookId = IntPtr.Zero;
        
        private CallbackDelegate MouseHook = null;
        private CallbackDelegate KeyboardHook = null;
        public event LocalKeyEventHandler KeyDown;
        public event LocalKeyEventHandler KeyUp;
        public event ErrorEventHandler OnError;
        public event LocalMouseMoveHandler OnMouseMove;
        public event LocalMouseEventHandler OnMouseClick;
        public event LocalMouseEventDown OnMouseDown;
        public event LocalMouseEventUp OnMouseUp;

        private delegate int CallbackDelegate(int Code, IntPtr W, IntPtr L);

        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr SetWindowsHookEx(HookType idHook, CallbackDelegate lpfn, IntPtr hInstance, int threadId);
        
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);
        #region Construction/Deconstruction
        public UIReader(bool Global, string[] LoggingWindows) :
            base(Global, LoggingWindows) {

        }

        public bool GenerateHook(HookTypePub PubHook) {
            if (LoggingWindows.Length < 1) return false;

            int Type;
            if (PubHook == HookTypePub.Keyboard) {
                if(KeyboardHook != null) {
                    return false;
                }
                KeyboardHook = new CallbackDelegate(KeybHookProc);
                Type      = (int)HookType.WH_KEYBOARD;
                _keyboardHookId = ConnectHook(KeyboardHook);
            } else {
                if (MouseHook != null) {
                    return false;
                }
                MouseHook = new CallbackDelegate(MouseHookProc);
                Type      = (int)HookType.WH_MOUSE;
                _mouseHookId = ConnectHook(MouseHook);
            }
            
            return true;
            IntPtr ConnectHook(CallbackDelegate callback) {
                if (Global) {
                    IntPtr hInstance = LoadLibrary("User32");
                    return SetWindowsHookEx((HookType)PubHook, callback,
                        hInstance,
                        0);
                } else {
                    return SetWindowsHookEx((HookType)Type, callback,
                        IntPtr.Zero,
                        GetCurrentThreadId());
                }
            }
        }
        public bool DisconnectHook(HookTypePub PubHook) {
            if (PubHook == HookTypePub.Keyboard) {
                if (KeyboardHook == null) { //Disconnecting an empty hook should still succeed?
                    return true;
                }
                if(UnhookWindowsHookEx(_keyboardHookId) is false) {
                    return false;
                }
                KeyboardHook = null;
                _keyboardHookId = IntPtr.Zero;
            } else {
                if (MouseHook == null) {
                    return true;
                }
                if(UnhookWindowsHookEx(_mouseHookId) is false){
                    return false;
                }
                MouseHook = null;
                _mouseHookId = IntPtr.Zero;
            }
            return true;
        }

        
        bool IsFinalized = false;
        ~UIReader() {
            if (IsFinalized) return;
            UnhookWindowsHookEx(_mouseHookId);
            UnhookWindowsHookEx(_keyboardHookId);
            IsFinalized = true;
        }
        public new void Dispose() {
            if (IsFinalized) return;
            UnhookWindowsHookEx(_mouseHookId);
            UnhookWindowsHookEx(_keyboardHookId);
            IsFinalized = true;
        }
        #endregion Construction/Deconstruction
        private bool SafetyChecks(int Code) {
            //Check for window within allowed options
            string ActiveWindow = WindowInfo.GetActiveWindowTitle();
            if (ActiveWindow == null || Code < 0) {
                return false;
            }
            //Inaccurate Check
            if (Global) {
                if (!LoggingWindows.Any(x => ActiveWindow.Contains(x))) {
                    return false;
                }
            }
            //EXACT Check
            else if (!LoggingWindows.Any(x => x.Contains(ActiveWindow))) {
                return false;
            }
            return true;
        }
        //Mouse Documentation
        //https://learn.microsoft.com/en-us/windows/win32/winmsg/lowlevelmouseproc
        [MTAThread]
        private int MouseHookProc(int Code, IntPtr W, IntPtr L) {
            if (!SafetyChecks(Code)) {
                return CallNextHookEx(_mouseHookId, Code, W, L);
            }

            bool ButtonDirection = false;
            try {
                MouseButtons ButtonClicked = MouseButtons.None;
                MouseEvents Event = (MouseEvents)W;
                //https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msllhookstruct
                MouseInput input = Marshal.PtrToStructure<MouseInput>(L);
                
                switch (Event) {
                    case MouseEvents.MouseMove:
                        OnMouseMove?.Invoke(input.dx, input.dy);
                        return CallNextHookEx(_mouseHookId, Code, W, L);
                    case MouseEvents.MouseClickLeftDown:
                        ButtonClicked   = MouseButtons.Left;
                        ButtonDirection = true;
                        break;
                    case MouseEvents.MouseClickLeftUp:
                        ButtonClicked = MouseButtons.Left;
                        break;
                    case MouseEvents.MouseClickRightDown:
                        ButtonClicked   = MouseButtons.Right;
                        ButtonDirection = true;
                        break;
                    case MouseEvents.MouseClickRightUp:
                        ButtonClicked = MouseButtons.Right;
                        break;
                    case MouseEvents.MouseScrollClick:
                        ButtonClicked   = MouseButtons.Middle;
                        ButtonDirection = true;
                        break;
                    case MouseEvents.MouseScroll:
                        ButtonClicked = MouseButtons.None;
                        break;
                    default:
                        return CallNextHookEx(_mouseHookId, Code, W, L);
                }

                if (ButtonDirection) {
                    OnMouseDown?.Invoke(ButtonClicked);
                }
                else {
                    OnMouseUp?.Invoke(ButtonClicked);
                }
                OnMouseClick?.Invoke(ButtonClicked);
                //Mouse Data->ScrollDown: 4287102976
                //Mouse Data->ScrollUp:   7864320
            } catch (Exception e) {
                OnError?.Invoke(e);
                //Dont talk bout no errors :)
            }
            return CallNextHookEx(_mouseHookId, Code, W, L);
        }

        [MTAThread]
        //The listener that will trigger events
        private int KeybHookProc(int Code, IntPtr W, IntPtr L) {
            if (!SafetyChecks(Code)) {
                return CallNextHookEx(_keyboardHookId, Code, W, L);
            }

            try {
                if (Global) {
                    KeyEvents kEvent = (KeyEvents)W;

                    int vkCode = Marshal.ReadInt32(L); //Leser vkCode som er de første 32 bits hvor L peker.
                                                       //Reads vkCode which is the first 32 bits where L points.
                    if (kEvent != KeyEvents.KeyDown
                    && kEvent != KeyEvents.KeyUp
                    && kEvent != KeyEvents.SKeyDown
                    && kEvent != KeyEvents.SKeyUp) {
                    }
                    if (kEvent == KeyEvents.KeyDown
                    || kEvent == KeyEvents.SKeyDown) {
                        KeyDown?.Invoke((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    }
                    if (kEvent == KeyEvents.KeyUp
                    || kEvent == KeyEvents.SKeyUp) {
                        KeyUp?.Invoke((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    }
                } else if (Code == 3) {
                    int keydownup = L.ToInt32() >> 30;  //Magic Bitshift
                    if (keydownup == 0) {
                        KeyDown?.Invoke((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    } else if (keydownup == -1) {
                        KeyUp?.Invoke((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    }
                }
            } catch (Exception e) {
                //Pass Error upwards to error handling.
                OnError?.Invoke(e);
            }
            return CallNextHookEx(_keyboardHookId, Code, W, L);
        }
        #region EventEnums
        private enum MouseEvents {
            MouseMove = 0x0200,
            MouseClickLeftDown = 0x0201,
            MouseClickLeftUp = 0x0202,
            MouseClickRightDown = 0x0204,
            MouseClickRightUp = 0x0205,
            MouseScrollClick = 0x0207,
            MouseScroll = 0x020a
        }
        private enum KeyEvents {
            KeyDown = 0x0100,
            KeyUp = 0x0101,
            SKeyDown = 0x0104,
            SKeyUp = 0x0105
        }
        private enum HookType : int {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }
        public enum HookTypePub {
            Keyboard = HookType.WH_KEYBOARD_LL,
            Mouse = HookType.WH_MOUSE_LL
        }
        #endregion EventEnums
        #region KeyStates
        [DllImport("user32.dll")]
        private static extern short GetKeyState(Keys nVirtKey);
        public static bool GetCapslock() {
            return Convert.ToBoolean(GetKeyState(Keys.CapsLock)) & true;
        }
        public static bool GetNumlock() {
            return Convert.ToBoolean(GetKeyState(Keys.NumLock)) & true;
        }
        public static bool GetScrollLock() {
            return Convert.ToBoolean(GetKeyState(Keys.Scroll)) & true;
        }
        public static bool GetHomePressed() {
            int state = GetKeyState(Keys.LWin) + GetKeyState(Keys.RWin);
            if (state > 1 || state < -1)
                return true;
            return false;
        }
        public static bool GetShiftPressed() {
            int state = GetKeyState(Keys.ShiftKey);
            if (state > 1 || state < -1)
                return true;
            return false;
        }
        public static bool GetCtrlPressed() {
            int state = GetKeyState(Keys.ControlKey);
            if (state > 1 || state < -1)
                return true;
            return false;
        }
        public static bool GetAltPressed() {
            int state = GetKeyState(Keys.Menu);
            if (state > 1 || state < -1)
                return true;
            return false;
        }
        #endregion KeyStates
    }
}