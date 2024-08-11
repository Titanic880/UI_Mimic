using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using WinApi.User32;
using System;

namespace UI_Mimic.Windows {
    public static class UIControl {
        public static bool Safety { get => safety; }
        private static bool safety = true;
        /// <summary>
        /// Allows the user to enable the use of InputDown/Up without Auto releasing
        /// </summary>
        /// <param name="Input3040312"></param>
        /// <param name="SafetyState"></param>
        /// <returns></returns>
        public static string ToggleKeyInputSafety(int Input3040312, bool SafetyState) {
            if (Input3040312 != 3040312) {
                return $"{Safety}: Safety has not been turned off";
            } else {
                safety = SafetyState;
                return $"{Safety}: Safety has been placed into the: {SafetyState} setting";
            }
        }
        
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
        [DllImport("user32.dll")]
        private static extern short VkKeyScanExA(char ch, IntPtr dwhkl);
        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        public static void Mouse_Move_Click(Point NewPosition, bool LeftClick = true) {
            Input[] inputs = Builder_MouseInput(LeftClick: LeftClick);
            Cursor.Position = NewPosition;
            _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
        public static void Mouse_Move(Point NewPosition) {
            Cursor.Position = NewPosition;
        }
        public static void Mouse_Click(bool LeftClick) {
            Input[] inputs = Builder_MouseInput(LeftClick: LeftClick);
            _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
        private static Input[] Builder_MouseInput(bool MoveOnly = false, bool LeftClick = true) {
            Input[] ret = new Input[2];
            ret[0] = new Input {
                type = (int)InputType.Mouse,
                u = new InputUnion {
                    mi = new MouseInput {
                        dwFlags = (uint)(LeftClick ? MouseEventF.LeftDown : MouseEventF.RightDown)
                    }
                }
            };
            ret[1] = new Input {
                type = (int)InputType.Mouse,
                u = new InputUnion {
                    mi = new MouseInput {
                        dwFlags = (uint)(LeftClick ? MouseEventF.LeftUp : MouseEventF.RightUp),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };
            return ret;
        }

        public static VirtualKey GetKey(char input) {
            short value = VkKeyScanExA(input,GetKeyboardLayout(0));
            return (VirtualKey)value;
        }
        public static void KeyStrokeSim(VirtualKey vk) {
            Input[] val ={
                KeyStroke_Builder(vk, true),
                KeyStroke_Builder(vk, false)
            };
            _ = SendInput((uint)val.Length, val, Marshal.SizeOf(typeof(Input)));
        }
        /// <summary>
        /// Requires setting the Safety Value to false via ToggleKeyInputSafety method
        /// </summary>
        /// <param name="vk"></param>
        public static void KeyStrokeDown(VirtualKey vk) {
            if(Safety) {
                return;
            }
            Input[] val = { KeyStroke_Builder(vk,true) };
            _ = SendInput((uint)val.Length, val, Marshal.SizeOf(typeof(Input)));
        }
        /// <summary>
        /// Requires setting the Safety Value to false via ToggleKeyInputSafety method
        /// </summary>
        /// <param name="vk"></param>
        public static void KeyStrokeUp(VirtualKey vk) {
            if (Safety) {
                return;
            }
            Input[] val = { KeyStroke_Builder(vk,false) };
            _ = SendInput((uint)val.Length, val, Marshal.SizeOf(typeof(Input)));
        }
        public static void KeyStrokeArrSim(VirtualKey[] vks) {
            foreach (VirtualKey key in vks) {
                Input[] values = {
                    KeyStroke_Builder(key, true),
                    KeyStroke_Builder(key,false)
                };
                _ = SendInput((uint)values.Length, values, Marshal.SizeOf(typeof(Input)));
            }
        }
        public static void KeyStrokesFromString(string InputText) {
            foreach (char a in InputText) {
                Input[] values = {
                    KeyStroke_Builder(Structures.CharCodes[a], true),
                    KeyStroke_Builder(Structures.CharCodes[a],false)
                };
                _ = SendInput((uint)values.Length, values, Marshal.SizeOf(typeof(Input)));
            }
        }
        private static Input KeyStroke_Builder(VirtualKey vk, bool KeyUp) {
            return new Input {
                type = (int)InputType.Keyboard,
                u = new InputUnion {
                    ki = new KeyboardInput {
                        wVk = (ushort)vk,
                        dwFlags = (uint)(KeyUp ? KeyEventF.KeyUp : KeyEventF.KeyDown),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };
        }
        private static Input KeyStroke_Builder(ushort value, bool KeyUp) {
            return new Input {
                type = (int)InputType.Keyboard,
                u = new InputUnion {
                    ki = new KeyboardInput {
                        wVk = 0,
                        wScan = value,
                        dwFlags = (uint)(KeyUp ? KeyEventF.KeyUp : KeyEventF.KeyDown | KeyEventF.Unicode),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };
        }
    }
}
