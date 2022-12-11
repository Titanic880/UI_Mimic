using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System;
using System.Diagnostics;

namespace UI_Mimic
{
    public static class UserControl
    {
        #region Imports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);
        #endregion Imports


        #region Public Interface
        public static void FocusProcess(string ProcessName)
        {
            Process[] allitems = Process.GetProcesses();
            foreach (Process a in allitems)
                if (a.ProcessName.ToLower().Contains("whatsminer"))
                {
                    IntPtr hWnd = a.MainWindowHandle;
                    ////ShowWindowAsync(new HandleRef(null, hWnd), SW_RESTORE);
                    SetForegroundWindow(hWnd);
                }
        }
        /// <summary>
        /// Move the mouse pointer then click on the spot
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="LeftClick"></param>
        public static void Mouse_Move_Click(Point mousePosition, bool LeftClick = true)
        {
            Cursor.Position = mousePosition;
            Input[] MouseAction = new Input[]
            {
            new Input
            {
                type = (int)InputType.Mouse,
                u = new InputUnion
                {
                    mi = new MouseInput
                    {
                        dwFlags = (uint) (LeftClick ? MouseEventF.LeftDown : MouseEventF.RightDown)
                    }
                }
            },
            new Input
            {
                type = (int)InputType.Mouse,
                u = new InputUnion
                {
                    mi = new MouseInput
                    {
                    dwFlags = (uint)(LeftClick ? MouseEventF.LeftUp : MouseEventF.RightUp),
                    dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
            };
            _ = SendInput((uint)MouseAction.Length, MouseAction, Marshal.SizeOf(typeof(Input)));
        }
        /// <summary>
        /// Move the Mouse Cursor
        /// </summary>
        /// <param name="point"></param>
        public static void Mouse_Move(Point point)
        {
            Input[] MouseAction = new Input[]
            {
                new Input
                {
                    type = (int)InputType.Mouse,
                    u = new InputUnion
                    {
                        mi = new MouseInput
                        {
                            dx = point.X,
                            dy = point.Y,
                            dwFlags = (uint)MouseEventF.Move
                        }
                    }
                }
            };
            _ = SendInput((uint)MouseAction.Length, MouseAction, Marshal.SizeOf(typeof(Input)));
        }
        /// <summary>
        /// Click the Mouse Cursor (False for Right Click)
        /// </summary>
        public static void Mouse_Click(bool LeftClick = true)
        {
            Input[] MouseLeft = new Input[] {
            new Input
            {
                type = (int)InputType.Mouse,
                u = new InputUnion
                {
                    mi = new MouseInput
                    {
                        dwFlags = (uint)(MouseEventF.Move | (LeftClick ? MouseEventF.LeftDown : MouseEventF.RightDown) )
                    }
                }
            },
            new Input
            {
                type = (int)InputType.Mouse,
                u = new InputUnion
                {
                    mi = new MouseInput
                    {
                    dwFlags = (uint)(LeftClick ? MouseEventF.LeftUp : MouseEventF.RightUp),
                    dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
        };
            _ = SendInput((uint)MouseLeft.Length, MouseLeft, Marshal.SizeOf(typeof(Input)));
        }
        /// <summary>
        /// Types out the provided String
        /// </summary>
        /// <param name="Send"></param>
        public static void Keyboard_Text(string Send)
        {
            List<Input> inputs = new List<Input>();
            for (int i = 0; i < Send.Length; i++)
            {
                inputs.Add(GenerateKeyEvent(CharCodes[Send[i]], false));
                inputs.Add(GenerateKeyEvent(CharCodes[Send[i]]));
            }
            _ = SendInput((uint)inputs.ToArray().Length, inputs.ToArray(), Marshal.SizeOf(typeof(Input)));
        }
        /// <summary>
        /// Sends the provided Key code
        /// </summary>
        public static void Keyboard_Code(WinApi.User32.ScanCodes input)
        {
            //WinApi.User32.VirtualKey.RETURN;
            //wScan =  (ushort)WinApi.User32.ScanCodes.RETURN, ///Enter Key Code 0x1C
            Input[] inputs = new Input[]
            {
            ///Key Down
            new Input
            {
                type = (int)InputType.Keyboard,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = 0,
                        wScan = ((ushort)input), ///Enter Key Code
                        dwFlags = (uint)(KeyEventF.KeyDown | KeyEventF.Scancode),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            },
            ///Key Up
            new Input
            {
                type = (int)InputType.Keyboard,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = 0,
                        wScan = ((ushort)input), ///Enter Key Code
                        dwFlags = (uint)(KeyEventF.KeyUp | KeyEventF.Scancode),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
            };
            _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
        #endregion Public Interface


        #region Private Data
        private static Input GenerateKeyEvent(ushort Code, bool Up = true)
        {
            return new Input
            {
                type = (int)InputType.Keyboard,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = 0,
                        wScan = (ushort)Code,
                        dwFlags = (uint)(Up ? KeyEventF.KeyUp : KeyEventF.KeyDown | KeyEventF.Unicode),
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            };
        }
        /// <summary>
        /// All Text character Codes in a Dictionary
        /// </summary>
        private static readonly Dictionary<char, ushort> CharCodes = new Dictionary<char, ushort>()
    {
            //Special Charactors
        {' ',0x20 },
        {'!',0x21 },
        {'"',0x22 },
        {'#',0x23 },
        {'$',0x24 },
        {'%',0x25 },
        {'&',0x26 },
        {"'".ToCharArray()[0],0x27 },
        {'(',0x28 },
        {')',0x29 },
        {'*',0x2A },
        {'+',0x2B },
        {',',0x2C },
        {'-',0x2D },
        {'.',0x2E },
        {'/',0x2F },
        //Numbers
        {'0',0x30 },
        {'1',0x31 },
        {'2',0x32 },
        {'3',0x33 },
        {'4',0x34 },
        {'5',0x35 },
        {'6',0x36 },
        {'7',0x37 },
        {'8',0x38 },
        {'9',0x39 },
         //Special Characters
        {':',0x3A },
        {';',0x3B },
        {'<',0x3C },
        {'=',0x3D },
        {'>',0x3E },
        {'?',0x3F },
        {'@',0x40 },
        //Upper case
        {'A',0x41 },
        {'B',0x42 },
        {'C',0x43 },
        {'D',0x44 },
        {'E',0x45 },
        {'F',0x46 },
        {'G',0x47 },
        {'H',0x48 },
        {'I',0x49 },
        {'J',0x4A },
        {'K',0x4B },
        {'L',0x4C },
        {'M',0x4D },
        {'N',0x4E },
        {'O',0x4F },
        {'P',0x50 },
        {'Q',0x51 },
        {'R',0x52 },
        {'S',0x53 },
        {'T',0x54 },
        {'U',0x55 },
        {'V',0x56 },
        {'W',0x57 },
        {'X',0x58 },
        {'Y',0x59 },
        {'Z',0x5A },
        //Special Characters
        {'[',0x5B },
        {'\\',0x5C },
        {']',0x5D },
        {'^',0x5E },
        {'_',0x5F },
        {'`',0x60 },
        //Lower case
        {'a',0x61 },
        {'b',0x62 },
        {'c',0x63 },
        {'d',0x64 },
        {'e',0x65 },
        {'f',0x66 },
        {'g',0x67 },
        {'h',0x68 },
        {'i',0x69 },
        {'j',0x6A },
        {'k',0x6B },
        {'l',0x6C },
        {'m',0x6D },
        {'n',0x6E },
        {'o',0x6F },
        {'p',0x70 },
        {'q',0x71 },
        {'r',0x72 },
        {'s',0x73 },
        {'t',0x74 },
        {'u',0x75 },
        {'v',0x76 },
        {'w',0x77 },
        {'x',0x78 },
        {'y',0x79 },
        {'z',0x7A },
        {'{',0x7B },
        {'|',0x7C },
        {'}',0x7D },
        {'~',0x7E },
     };
        #endregion Private Data
    }
}
