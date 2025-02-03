using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Linq;
using System;

namespace UI_Mimic.Windows {
    /// <summary>
    /// Taken and modified for use from this post.
    /// https://stackoverflow.com/questions/34281223/c-sharp-hook-global-keyboard-events-net-4-0
    /// </summary>
    internal class W_UIReader : InputReader {

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
        public W_UIReader(bool Global, string[] LoggingWindows) :
            base(Global, LoggingWindows) {
        }


        public bool Debug_Feature_01_SetReplace(Debug_Feature_01_MultiTypeStorage obj) {
            if (Debug_Feature_01_LockReplacement) {
                return false;
            }
            Debug_Feature_01_Replacement_Replace = obj;
            return true;
        }
        public bool Debug_Feature_01_SetTarget(Debug_Feature_01_MultiTypeStorage obj) {
            if(Debug_Feature_01_LockReplacement) {
                return false;
            }
            Debug_Feature_01_Replacement_Target = obj;
            return true;
        }
        public bool Debug_Feature_01_SetReplacementLock() {
            if( Debug_Feature_01_LockReplacement) {
                return false;
            }
            Debug_Feature_01_LockReplacement = true;
            return true;
        }

        public override bool GenerateHook(HookTypePub PubHook) {
            if (LoggingWindows.Length < 1) return false;

            int Type;
            if (PubHook == HookTypePub.Keyboard) {
                if (KeyboardHook != null) {
                    return false;
                }
                KeyboardHook = new CallbackDelegate(KeybHookProc);
                Type = (int)HookTypePub.Keyboard;
                _keyboardHookId = ConnectHook(KeyboardHook);
            } else if (PubHook == HookTypePub.Mouse){
                if (MouseHook != null) {
                    return false;
                }
                MouseHook = new CallbackDelegate(MouseHookProc);
                Type = (int)HookTypePub.Mouse;
                _mouseHookId = ConnectHook(MouseHook);
            }
            else if(PubHook == HookTypePub.Debug_Feature_01_Replacement) {
                if(Debug_Feature_01_Replacement_CBD != null) {
                    return false;
                }
                Debug_Feature_01_Replacement_CBD = new CallbackDelegate(ReplacementHookProc);
                Type = (int)HookType.WH_KEYBOARD;
                _Debug_Feature_01_Replacement_IntPtr[0] = ConnectHook(Debug_Feature_01_Replacement_CBD);
                Type = (int)HookTypePub.Mouse;
                _Debug_Feature_01_Replacement_IntPtr[1] = ConnectHook(Debug_Feature_01_Replacement_CBD);
                Debug_Feature_01_LockReplacement = false;
                return true;
            } else {
                return false;
            }

            return true;
            IntPtr ConnectHook(CallbackDelegate callback) {
                if (Global) {
                    IntPtr hInstance = LoadLibrary("User32");
                    return SetWindowsHookEx((HookType)Type, callback,
                        hInstance,
                        0);
                } else {
                    return SetWindowsHookEx((HookType)Type, callback,
                        IntPtr.Zero,
                        GetCurrentThreadId());
                }
            }
        }
        public override bool DisconnectHook(HookTypePub PubHook) {
            if (PubHook == HookTypePub.Keyboard) {
                if (KeyboardHook == null) { //Disconnecting an empty hook should still succeed?
                    return true;
                }
                if (UnhookWindowsHookEx(_keyboardHookId) is false) {
                    return false;
                }
                KeyboardHook = null;
                _keyboardHookId = IntPtr.Zero;
            } else if (PubHook == HookTypePub.Mouse) {
                if (MouseHook == null) {
                    return true;
                }
                if (UnhookWindowsHookEx(_mouseHookId) is false) {
                    return false;
                }
                MouseHook = null;
                _mouseHookId = IntPtr.Zero;
            } else if (PubHook == HookTypePub.Debug_Feature_01_Replacement) {
                if(Debug_Feature_01_Replacement_CBD == null) {
                    return true;
                }
                if(UnhookWindowsHookEx(_Debug_Feature_01_Replacement_IntPtr[0]) is false) {
                    return false;
                }
                if (UnhookWindowsHookEx(_Debug_Feature_01_Replacement_IntPtr[1]) is false) {
                    return false;
                }
                Debug_Feature_01_LockReplacement = false;
                Debug_Feature_01_Replacement_CBD = null;
                _Debug_Feature_01_Replacement_IntPtr = new IntPtr[2];
            } else {
                return false;
            }
            return true;
        }
        bool IsFinalized = false;
        ~W_UIReader() {
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
            base.Dispose();
        }
        #endregion Construction/Deconstruction
        /// <summary>
        /// Checks Global Allowed AND Window is in the approved list
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        private bool SafetyChecks(int Code) {
            //Check for window within allowed options
            string ActiveWindow = W_WindowInfo.GetActiveWindowTitle();
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
        private int MouseHookProc(int Code, IntPtr EventPtr, IntPtr InputPtr) {
            if (SafetyChecks(Code) is false) {
                return CallNextHookEx(_mouseHookId, Code, EventPtr, InputPtr);
            }

            bool ButtonDirection = false;
            try {
                MouseButtons ButtonClicked = MouseButtons.None;
                MouseEvents Event = (MouseEvents)EventPtr; //Marshal Ptr conversion throws `System.ArgumentException: 'The specified structure must be blittable or have layout information.'`

                //https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msllhookstruct
                MouseInput input = Marshal.PtrToStructure<MouseInput>(InputPtr);

                switch (Event) {
                case MouseEvents.MouseMove:
                    TriggerOnMouseMove(input.dx, input.dy);
                    return CallNextHookEx(_mouseHookId, Code, EventPtr, InputPtr);
                case MouseEvents.MouseClickLeftDown:
                    ButtonClicked = MouseButtons.Left;
                    ButtonDirection = true;
                    break;
                case MouseEvents.MouseClickLeftUp:
                    ButtonClicked = MouseButtons.Left;
                    break;
                case MouseEvents.MouseClickRightDown:
                    ButtonClicked = MouseButtons.Right;
                    ButtonDirection = true;
                    break;
                case MouseEvents.MouseClickRightUp:
                    ButtonClicked = MouseButtons.Right;
                    break;
                case MouseEvents.MouseScrollClick:
                    ButtonClicked = MouseButtons.Middle;
                    ButtonDirection = true;
                    break;
                case MouseEvents.MouseScroll:
                    ButtonClicked = MouseButtons.None;
                    break;
                default:
                    return CallNextHookEx(_mouseHookId, Code, EventPtr, InputPtr);
                }

                if (ButtonDirection) {
                    TriggerOnMouseDown(ButtonClicked);
                } else {
                    TriggerOnMouseUp(ButtonClicked);
                }
                TriggerOnMouseClick(ButtonClicked);

                //Mouse Data->ScrollDown: 4287102976
                //Mouse Data->ScrollUp:   7864320
            } catch (Exception e) {
                TriggerOnError(e);
                //Dont talk bout no errors :)
            }
            return CallNextHookEx(_mouseHookId, Code, EventPtr, InputPtr);
        }
        [MTAThread]
        private int KeybHookProc(int Code, IntPtr W, IntPtr L) {
            if (SafetyChecks(Code) is false) {
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
                        TriggerKeyDown((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    }
                    if (kEvent == KeyEvents.KeyUp
                    || kEvent == KeyEvents.SKeyUp) {
                        TriggerKeyUp((Keys)vkCode, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    }
                } else if (Code == 3) {
                    int keydownup = L.ToInt32() >> 30;  //Zero out all but most significate 2 bits (negative should be impossible...?)
                    if (keydownup == 0) {
                        TriggerKeyDown((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    } else if (keydownup == -1) {
                        TriggerKeyUp((Keys)W, GetShiftPressed(), GetCtrlPressed(), GetAltPressed(), GetHomePressed());
                    }
                }
            } catch (Exception e) {
                //Pass Error upwards to error handling.
                TriggerOnError(e);
            }
            return CallNextHookEx(_keyboardHookId, Code, W, L);
        }

        public int Debug_Replacements = 0;
        [MTAThread]
        private int ReplacementHookProc(int Code, IntPtr EventPtr, IntPtr InputPtr) {
            //Decode the incoming data so that we can return the proper Id
            Debug_Feature_01_MultiTypeStorage MTS = new Debug_Feature_01_MultiTypeStorage(Marshal.ReadInt32(EventPtr));
            IntPtr ReturnPtr = 
                MTS.KeyEvent == KeyEvents.None ? 
                _Debug_Feature_01_Replacement_IntPtr[0] : 
                _Debug_Feature_01_Replacement_IntPtr[1] ;
            
            if(MTS.MouseEvent == MouseEvents.None && MTS.KeyEvent == KeyEvents.None || MTS.MouseEvent == MouseEvents.MouseMove) {
                return CallNextHookEx(ReturnPtr,Code,EventPtr,InputPtr);
            }
            if (SafetyChecks(Code) == false || Debug_Feature_01_Replacement_Target.Debug_TrueValue == default || Debug_Feature_01_Replacement_Replace.Debug_TrueValue == default) {
                return CallNextHookEx(ReturnPtr, Code, EventPtr, InputPtr);
            }
            if (Debug_Feature_01_Replacement_Target.Equals(MTS) == false) {
                return CallNextHookEx(ReturnPtr, Code, EventPtr, InputPtr);
            }
            if (Debug_Feature_01_Replacement_Replace.Equals(MTS)) {
                return CallNextHookEx(ReturnPtr, Code, EventPtr, InputPtr);
            }
            Debug_Replacements++;
            //Marshal.StructureToPtr(Debug_Feature_01_Replacement_Replace.Debug_TrueValue, EventPtr, true);
            return CallNextHookEx(_keyboardHookId, Code, EventPtr, InputPtr);
        }

        /*[MTAThread]
        private int GamepadHookProc(int Code, IntPtr W, IntPtr L) {
            if (SafetyChecks(Code) is false) {
                return CallNextHookEx(_mouseHookId, Code,W,L);
            }



            return CallNextHookEx(_mouseHookId, Code, W, L);
        }*/
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