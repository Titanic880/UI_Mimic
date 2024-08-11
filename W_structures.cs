using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using WinApi.User32;

namespace UI_Mimic {
    public enum HookTypePub {
        Keyboard = HookType.WH_KEYBOARD_LL,
        Mouse = HookType.WH_MOUSE_LL
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseInput {
        internal int dx;
        internal int dy;
        internal uint mouseData;
        internal uint dwFlags;
        internal uint time;
        internal IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyboardInput {
        internal ushort wVk;
        internal ushort wScan;
        internal uint dwFlags;
        internal uint time;
        internal IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct HardwareInput {
        internal uint uMsg;
        internal ushort wParamL;
        internal ushort wParamH;
    }
    [StructLayout(LayoutKind.Explicit)]
    internal struct InputUnion {
        [FieldOffset(0)] internal MouseInput mi;
        [FieldOffset(0)] internal KeyboardInput ki;
        [FieldOffset(0)] internal HardwareInput hi;
    }
    internal struct Input {
        internal int type;
        internal InputUnion u;
    }

    [Flags]
    internal enum InputType {
        Mouse = 0,
        Keyboard = 1,
        Hardware = 2
    }
    [Flags]
    internal enum KeyEventF {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008
    }
    [Flags]
    internal enum MouseEventF {
        Absolute = 0x8000,
        HWheel = 0x01000,
        Move = 0x0001,
        MoveNoCoalesce = 0x2000,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        VirtualDesk = 0x4000,
        Wheel = 0x0800,
        XDown = 0x0080,
        XUp = 0x0100
    }
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
    internal readonly struct Structures {
        /// <summary>
        /// All Text character Codes in a Dictionary
        /// </summary>
        internal static readonly Dictionary<char, ushort> CharCodes = new Dictionary<char, ushort>()
    {
        //Special Charactors
        {'\n',0x1C },   //Return/Enter
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
        //Special Characters
        {'{',0x7B },
        {'|',0x7C },
        {'}',0x7D },
        {'~',0x7E }
     };
    }
}
