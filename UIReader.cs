﻿using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System;
using WinApi.User32;
using WinApi.Windows;

namespace UI_Mimic {
    /// <summary>
    /// Taken and modified for use from this post.
    /// https://stackoverflow.com/questions/34281223/c-sharp-hook-global-keyboard-events-net-4-0
    /// </summary>
    public class UIReader : IDisposable {
        private readonly bool Global = false;
        private readonly IntPtr HookID = IntPtr.Zero;
        private readonly CallbackDelegate TheHookCB = null;
        private readonly string[] LoggingWindows;

        public delegate void ErrorEventHandler(Exception e);
        public delegate void LocalKeyEventHandler(Keys key, bool Shift, bool Ctrl, bool Alt, bool Home);
        //public delegate void LocalKeyEventHandler_Expanded(Keys key, bool Shift, bool Ctrl, bool Alt, bool Home);
        public event LocalKeyEventHandler KeyDown;
        public event LocalKeyEventHandler KeyUp;
        public event ErrorEventHandler OnError;

        public delegate void LocalMouseMoveHandler(int xPos, int yPos);
        public event LocalMouseMoveHandler OnMouseMove;
        public delegate void LocalMouseEventHandler(MouseButtons MouseAction);
        public event LocalMouseEventHandler OnMouseClick;

        public delegate int CallbackDelegate(int Code, IntPtr W, IntPtr L);

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

        internal enum HookType : int {
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

        //Only Expose the types of hooks DLL is setup for
        public enum HookTypePub {
            Keyboard = HookType.WH_KEYBOARD_LL,
            Mouse = HookType.WH_MOUSE_LL,   //NEW ADDITION: NOT FOR PUBLIC RELEASE YET
        }


        //Start hook
        public UIReader(bool Global, string[] LoggingWindows) {
            this.LoggingWindows = LoggingWindows;
            this.Global = Global;
            TheHookCB = new CallbackDelegate(KeybHookProc);

            if (Global) {
                IntPtr hInstance = LoadLibrary("User32");
                HookID = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, TheHookCB,
                    hInstance, //0 for local hook. or hwnd to user32 for global
                    0); //0 for global hook. or thread for the hook
            } else {
                HookID = SetWindowsHookEx(HookType.WH_KEYBOARD, TheHookCB,
                    IntPtr.Zero, //0 for local hook. or hwnd to user32 for global
                    GetCurrentThreadId()); //0 for global hook. or thread for the hook
            }
        }
        /// <summary>
        /// DEBUG ONLY::: HAS CHANCE TO CRASH PC
        /// </summary>
        /// <param name="Global"></param>
        /// <param name="LoggingWindows"></param>
        /// <param name="HookType"></param>
        public UIReader(bool Global, string[] LoggingWindows, HookTypePub Hook) {
            this.LoggingWindows = LoggingWindows;
            this.Global = Global;
            int Type;
            if (Hook == HookTypePub.Keyboard) {
                TheHookCB = new CallbackDelegate(KeybHookProc);
                Type = ((int)HookType.WH_KEYBOARD);
            } else {
                TheHookCB = new CallbackDelegate(MouseHookProc);
                Type = ((int)HookType.WH_MOUSE);
            }

            if (Global) {
                IntPtr hInstance = LoadLibrary("User32");
                HookID = SetWindowsHookEx((HookType)Hook, TheHookCB,
                    hInstance,
                    0);
            } else {
                HookID = SetWindowsHookEx((HookType)Type, TheHookCB,
                    IntPtr.Zero,
                    GetCurrentThreadId());
            }
        }


        public void TestError() {
            OnError?.Invoke(new Exception("test"));
        }
        bool IsFinalized = false;
        ~UIReader() {
            if (!IsFinalized) {
                UnhookWindowsHookEx(HookID);
                IsFinalized = true;
            }
        }
        public void Dispose() {
            if (!IsFinalized) {
                UnhookWindowsHookEx(HookID);
                IsFinalized = true;
            }
        }
        //Mouse Documentation
        //https://learn.microsoft.com/en-us/windows/win32/winmsg/lowlevelmouseproc
        [STAThread]
        private int MouseHookProc(int Code, IntPtr W, IntPtr L) {
            //Check for window within allowed options
            string ActiveWindow = WindowInfo.GetActiveWindowTitle();
            if (ActiveWindow == null || Code < 0) {
                return CallNextHookEx(HookID, Code, W, L);
            }
            //Inaccurate Check
            if (Global) {
                if (!LoggingWindows.Where(x => ActiveWindow.Contains(x)).Any()) {
                    return CallNextHookEx(HookID, Code, W, L);
                }
            }
            //EXACT Check
            else if (!LoggingWindows.Where(x => x.Contains(ActiveWindow)).Any()) {
                return CallNextHookEx(HookID, Code, W, L);
            }

            try {
                MouseButtons ButtonClicked = MouseButtons.None;
                MouseEvents Event = (MouseEvents)W;
                //https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msllhookstruct
                MouseInput input = Marshal.PtrToStructure<MouseInput>(L);

                switch (Event) {
                case MouseEvents.MouseMove:
                    OnMouseMove(input.dx, input.dy);
                    return CallNextHookEx(HookID, Code, W, L);
                case MouseEvents.MouseClickLeftDown:
                    ButtonClicked = MouseButtons.Left;
                    break;
                case MouseEvents.MouseClickLeftUp:
                    ButtonClicked = MouseButtons.Left;
                    break;
                case MouseEvents.MouseClickRightDown:
                    ButtonClicked = MouseButtons.Right;
                    break;
                case MouseEvents.MouseClickRightUp:
                    ButtonClicked = MouseButtons.Left;
                    break;
                case MouseEvents.MouseScrollClick:
                    ButtonClicked = MouseButtons.Middle;
                    break;
                case MouseEvents.MouseScroll:
                    ButtonClicked = MouseButtons.None;
                    break;
                default:
                    return CallNextHookEx(HookID, Code, W, L);
                }
                OnMouseClick(ButtonClicked);
                //Mouse Data->ScrollDown: 4287102976
                //Mouse Data->ScrollUp:   7864320
            } catch (Exception e) {
                OnError?.Invoke(e);
                //Dont talk bout no errors :)
            }
            return CallNextHookEx(HookID, Code, W, L);
        }

        [STAThread]
        //The listener that will trigger events
        private int KeybHookProc(int Code, IntPtr W, IntPtr L) {
            //Check for window within allowed options
            string ActiveWindow = WindowInfo.GetActiveWindowTitle();
            if (ActiveWindow == null) {
                return CallNextHookEx(HookID, Code, W, L);
            }

            if (Global) {
                //Inaccurate Check
                if (!LoggingWindows.Where(x => ActiveWindow.Contains(x)).Any()) {
                    return CallNextHookEx(HookID, Code, W, L);
                }
            }
            //EXACT Check
            else if (!LoggingWindows.Where(x => x.Contains(ActiveWindow)).Any()) {
                return CallNextHookEx(HookID, Code, W, L);
            } else if (Code < 0) {
                return CallNextHookEx(HookID, Code, W, L);
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
                    IntPtr ptr = IntPtr.Zero;

                    int keydownup = L.ToInt32() >> 30;
                    if (keydownup == 0) {
                        KeyDown?.Invoke((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    } else if (keydownup == -1) {
                        KeyUp?.Invoke((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    }
                    //System.Diagnostics.Debug.WriteLine("Down: " + (Keys)W);
                }
            } catch (Exception e) {
                OnError?.Invoke(e);
                //Ignore all errors...
            }

            return CallNextHookEx(HookID, Code, W, L);
        }

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
    }
}