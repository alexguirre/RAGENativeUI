namespace RAGENativeUI
{
    using System.Text;
    using System.Windows.Forms;

    using Rage;

    using RAGENativeUI.Internals;

    public static class InstructionalKeyExtensions
    {
        public static unsafe string GetId(this InstructionalKey key)
        {
            // special cases
            switch (key)
            {
                case InstructionalKey.ControllerDPadNone: return "b_8";
                case InstructionalKey.ControllerDPadAll: return "b_9";
                case InstructionalKey.ControllerDPadUpDown: return "b_10";
                case InstructionalKey.ControllerDPadLeftRight: return "b_11";
                case InstructionalKey.ControllerLStickRotate: return "b_20";
                case InstructionalKey.ControllerRStickRotate: return "b_29";
                case InstructionalKey.SymbolBusySpinner: return "b_44";
                case InstructionalKey.SymbolPlus: return "b_998";
                case InstructionalKey.SymbolArrowUp: return "b_0";
                case InstructionalKey.SymbolArrowDown: return "b_1";
                case InstructionalKey.SymbolArrowLeft: return "b_2";
                case InstructionalKey.SymbolArrowRight: return "b_3";
                case InstructionalKey.SymbolArrowUpDown: return "b_45";
                case InstructionalKey.SymbolArrowLeftRight: return "b_46";
                case InstructionalKey.SymbolArrowAll: return "b_47";
            }

            if (!CTextFormat.Available)
            {
                return "b_995"; // button with "???" in red
            }

            // from rage::ioMapperSource enum
            const uint IOMS_KEYBOARD = 0,
                       IOMS_MOUSE_ABSOLUTEAXIS = 1,
                       IOMS_MOUSE_WHEEL = 6,
                       IOMS_MOUSE_BUTTON = 7,
                       IOMS_PAD_DIGITALBUTTON = 9,
                       IOMS_PAD_AXIS = 12;

            const uint MouseAxisMask = 0x200,
                       MouseWheelMask = 0x400,
                       MouseButtonMask = 0x800,
                       ControllerAxisMask = 0x1000,
                       ControllerButtonMask = 0x2000;

            atFixedArray_sIconData_4 icons = default;
            uint parameter = (uint)key;
            uint source = (parameter & MouseAxisMask) != 0 ? IOMS_MOUSE_ABSOLUTEAXIS :
                          (parameter & MouseWheelMask) != 0 ? IOMS_MOUSE_WHEEL : 
                          (parameter & MouseButtonMask) != 0 ? IOMS_MOUSE_BUTTON :
                          (parameter & ControllerAxisMask) != 0 ? IOMS_PAD_AXIS :
                          (parameter & ControllerButtonMask) != 0 ? IOMS_PAD_DIGITALBUTTON :
                                                                    IOMS_KEYBOARD;

            if (source == IOMS_PAD_DIGITALBUTTON)
            {
                parameter = 1u << (int)(parameter & 0xFFu);
            }

            CTextFormat.GetInputSourceIcons(source, parameter, ref icons);

            const int BufferSize = 64;
            byte* buffer = stackalloc byte[BufferSize];
            CTextFormat.GetIconListFormatString(ref icons, buffer, BufferSize);

            return Encoding.UTF8.GetString(buffer, StrLen(buffer));

            static int StrLen(byte* str)
            {
                int len = 0;
                while (str[len] != 0) { len++; }
                return len;
            }
        }

        public static string GetInstructionalId(this Keys key) => key.GetInstructionalKey().GetId();

        public static InstructionalKey GetInstructionalKey(this Keys key)
            => key switch
            {
                Keys.LButton => InstructionalKey.MouseLeft,
                Keys.RButton => InstructionalKey.MouseRight,
                Keys.MButton => InstructionalKey.MouseMiddle,
                Keys.XButton1 => InstructionalKey.MouseExtra1,
                Keys.XButton2 => InstructionalKey.MouseExtra2,
                Keys.Back => InstructionalKey.Back,
                Keys.Tab => InstructionalKey.Tab,
                Keys.Return => InstructionalKey.Return,
                Keys.ShiftKey => InstructionalKey.LShift,
                Keys.ControlKey => InstructionalKey.LControl,
                Keys.Menu => InstructionalKey.LMenu,
                Keys.Pause => InstructionalKey.Pause,
                Keys.Capital => InstructionalKey.Capital,
                Keys.Escape => InstructionalKey.Escape,
                Keys.Space => InstructionalKey.Space,
                Keys.PageUp => InstructionalKey.PageUp,
                Keys.PageDown => InstructionalKey.PageDown,
                Keys.End => InstructionalKey.End,
                Keys.Home => InstructionalKey.Home,
                Keys.Left => InstructionalKey.Left,
                Keys.Up => InstructionalKey.Up,
                Keys.Right => InstructionalKey.Right,
                Keys.Down => InstructionalKey.Down,
                Keys.Snapshot => InstructionalKey.Snapshot,
                Keys.Insert => InstructionalKey.Insert,
                Keys.Delete => InstructionalKey.Delete,
                Keys.D0 => InstructionalKey.D0,
                Keys.D1 => InstructionalKey.D1,
                Keys.D2 => InstructionalKey.D2,
                Keys.D3 => InstructionalKey.D3,
                Keys.D4 => InstructionalKey.D4,
                Keys.D5 => InstructionalKey.D5,
                Keys.D6 => InstructionalKey.D6,
                Keys.D7 => InstructionalKey.D7,
                Keys.D8 => InstructionalKey.D8,
                Keys.D9 => InstructionalKey.D9,
                Keys.A => InstructionalKey.A,
                Keys.B => InstructionalKey.B,
                Keys.C => InstructionalKey.C,
                Keys.D => InstructionalKey.D,
                Keys.E => InstructionalKey.E,
                Keys.F => InstructionalKey.F,
                Keys.G => InstructionalKey.G,
                Keys.H => InstructionalKey.H,
                Keys.I => InstructionalKey.I,
                Keys.J => InstructionalKey.J,
                Keys.K => InstructionalKey.K,
                Keys.L => InstructionalKey.L,
                Keys.M => InstructionalKey.M,
                Keys.N => InstructionalKey.N,
                Keys.O => InstructionalKey.O,
                Keys.P => InstructionalKey.P,
                Keys.Q => InstructionalKey.Q,
                Keys.R => InstructionalKey.R,
                Keys.S => InstructionalKey.S,
                Keys.T => InstructionalKey.T,
                Keys.U => InstructionalKey.U,
                Keys.V => InstructionalKey.V,
                Keys.W => InstructionalKey.W,
                Keys.X => InstructionalKey.X,
                Keys.Y => InstructionalKey.Y,
                Keys.Z => InstructionalKey.Z,
                Keys.LWin => InstructionalKey.LWin,
                Keys.RWin => InstructionalKey.RWin,
                Keys.Apps => InstructionalKey.Apps,
                Keys.NumPad0 => InstructionalKey.NumPad0,
                Keys.NumPad1 => InstructionalKey.NumPad1,
                Keys.NumPad2 => InstructionalKey.NumPad2,
                Keys.NumPad3 => InstructionalKey.NumPad3,
                Keys.NumPad4 => InstructionalKey.NumPad4,
                Keys.NumPad5 => InstructionalKey.NumPad5,
                Keys.NumPad6 => InstructionalKey.NumPad6,
                Keys.NumPad7 => InstructionalKey.NumPad7,
                Keys.NumPad8 => InstructionalKey.NumPad8,
                Keys.NumPad9 => InstructionalKey.NumPad9,
                Keys.Multiply => InstructionalKey.Multiply,
                Keys.Add => InstructionalKey.Add,
                Keys.Subtract => InstructionalKey.Subtract,
                Keys.Decimal => InstructionalKey.Decimal,
                Keys.Divide => InstructionalKey.Divide,
                Keys.F1 => InstructionalKey.F1,
                Keys.F2 => InstructionalKey.F2,
                Keys.F3 => InstructionalKey.F3,
                Keys.F4 => InstructionalKey.F4,
                Keys.F5 => InstructionalKey.F5,
                Keys.F6 => InstructionalKey.F6,
                Keys.F7 => InstructionalKey.F7,
                Keys.F8 => InstructionalKey.F8,
                Keys.F9 => InstructionalKey.F9,
                Keys.F10 => InstructionalKey.F10,
                Keys.F11 => InstructionalKey.F11,
                Keys.F12 => InstructionalKey.F12,
                Keys.F13 => InstructionalKey.F13,
                Keys.F14 => InstructionalKey.F14,
                Keys.F15 => InstructionalKey.F15,
                Keys.F16 => InstructionalKey.F16,
                Keys.F17 => InstructionalKey.F17,
                Keys.F18 => InstructionalKey.F18,
                Keys.F19 => InstructionalKey.F19,
                Keys.F20 => InstructionalKey.F20,
                Keys.F21 => InstructionalKey.F21,
                Keys.F22 => InstructionalKey.F22,
                Keys.F23 => InstructionalKey.F23,
                Keys.F24 => InstructionalKey.F24,
                Keys.NumLock => InstructionalKey.NumLock,
                Keys.Scroll => InstructionalKey.Scroll,
                Keys.LShiftKey => InstructionalKey.LShift,
                Keys.RShiftKey => InstructionalKey.RShift,
                Keys.LControlKey => InstructionalKey.LControl,
                Keys.RControlKey => InstructionalKey.RControl,
                Keys.LMenu => InstructionalKey.LMenu,
                Keys.RMenu => InstructionalKey.RMenu,
                Keys.Oem1 => InstructionalKey.Oem1,
                Keys.Oemplus => InstructionalKey.Plus,
                Keys.Oemcomma => InstructionalKey.Comma,
                Keys.OemMinus => InstructionalKey.Minus,
                Keys.OemPeriod => InstructionalKey.Period,
                Keys.Oem2 => InstructionalKey.Oem2,
                Keys.Oem3 => InstructionalKey.Oem3,
                Keys.Oem4 => InstructionalKey.Oem4,
                Keys.Oem5 => InstructionalKey.Oem5,
                Keys.Oem6 => InstructionalKey.Oem6,
                Keys.Oem7 => InstructionalKey.Oem7,
                Keys.Oem102 => InstructionalKey.Oem102,
                _ => InstructionalKey.Unknown
            };

        public static string GetInstructionalId(this ControllerButtons button) => button.GetInstructionalKey().GetId();

        public static InstructionalKey GetInstructionalKey(this ControllerButtons button)
            => button switch
            {
                ControllerButtons.DPadUp => InstructionalKey.ControllerDPadUp,
                ControllerButtons.DPadDown => InstructionalKey.ControllerDPadDown,
                ControllerButtons.DPadLeft => InstructionalKey.ControllerDPadLeft,
                ControllerButtons.DPadRight => InstructionalKey.ControllerDPadRight,
                ControllerButtons.Start => InstructionalKey.ControllerStart,
                ControllerButtons.Back => InstructionalKey.ControllerBack,
                ControllerButtons.LeftThumb => InstructionalKey.ControllerLThumb,
                ControllerButtons.RightThumb => InstructionalKey.ControllerRThumb,
                ControllerButtons.LeftShoulder => InstructionalKey.ControllerLShoulder,
                ControllerButtons.RightShoulder => InstructionalKey.ControllerRShoulder,
                ControllerButtons.A => InstructionalKey.ControllerA,
                ControllerButtons.B => InstructionalKey.ControllerB,
                ControllerButtons.X => InstructionalKey.ControllerX,
                ControllerButtons.Y => InstructionalKey.ControllerY,
                _ => InstructionalKey.Unknown,
            };
    }

    public enum InstructionalKey // from rage::ioMapperParameter enum
    {
        Unknown = 0x0, // KEY_NULL
        Back = 0x8, // KEY_BACK
        Tab = 0x9, // KEY_TAB
        Return = 0xD, // KEY_RETURN
        Pause = 0x13, // KEY_PAUSE
        Capital = 0x14, // KEY_CAPITAL
        CapsLock = Capital,
        Escape = 0x1B, // KEY_ESCAPE
        Space = 0x20, // KEY_SPACE
        PageUp = 0x21, // KEY_PAGEUP
        Prior = PageUp, // KEY_PRIOR
        PageDown = 0x22, // KEY_PAGEDOWN
        Next = PageDown, // KEY_NEXT
        End = 0x23, // KEY_END
        Home = 0x24, // KEY_HOME
        Left = 0x25, // KEY_LEFT
        Up = 0x26, // KEY_UP
        Right = 0x27, // KEY_RIGHT
        Down = 0x28, // KEY_DOWN
        Snapshot = 0x2C, // KEY_SNAPSHOT
        PrintScreen = Snapshot,
        SysRq = Snapshot, // KEY_SYSRQ
        Insert = 0x2D, // KEY_INSERT
        Delete = 0x2E, // KEY_DELETE
        D0 = 0x30, // KEY_0
        D1 = 0x31, // KEY_1
        D2 = 0x32, // KEY_2
        D3 = 0x33, // KEY_3
        D4 = 0x34, // KEY_4
        D5 = 0x35, // KEY_5
        D6 = 0x36, // KEY_6
        D7 = 0x37, // KEY_7
        D8 = 0x38, // KEY_8
        D9 = 0x39, // KEY_9
        A = 0x41, // KEY_A
        B = 0x42, // KEY_B
        C = 0x43, // KEY_C
        D = 0x44, // KEY_D
        E = 0x45, // KEY_E
        F = 0x46, // KEY_F
        G = 0x47, // KEY_G
        H = 0x48, // KEY_H
        I = 0x49, // KEY_I
        J = 0x4A, // KEY_J
        K = 0x4B, // KEY_K
        L = 0x4C, // KEY_L
        M = 0x4D, // KEY_M
        N = 0x4E, // KEY_N
        O = 0x4F, // KEY_O
        P = 0x50, // KEY_P
        Q = 0x51, // KEY_Q
        R = 0x52, // KEY_R
        S = 0x53, // KEY_S
        T = 0x54, // KEY_T
        U = 0x55, // KEY_U
        V = 0x56, // KEY_V
        W = 0x57, // KEY_W
        X = 0x58, // KEY_X
        Y = 0x59, // KEY_Y
        Z = 0x5A, // KEY_Z
        LWin = 0x5B, // KEY_LWIN
        RWin = 0x5C, // KEY_RWIN
        Apps = 0x5D, // KEY_APPS
        NumPad0 = 0x60, // KEY_NUMPAD0
        NumPad1 = 0x61, // KEY_NUMPAD1
        NumPad2 = 0x62, // KEY_NUMPAD2
        NumPad3 = 0x63, // KEY_NUMPAD3
        NumPad4 = 0x64, // KEY_NUMPAD4
        NumPad5 = 0x65, // KEY_NUMPAD5
        NumPad6 = 0x66, // KEY_NUMPAD6
        NumPad7 = 0x67, // KEY_NUMPAD7
        NumPad8 = 0x68, // KEY_NUMPAD8
        NumPad9 = 0x69, // KEY_NUMPAD9
        Multiply = 0x6A, // KEY_MULTIPLY
        Add = 0x6B, // KEY_ADD
        Subtract = 0x6D, // KEY_SUBTRACT
        Decimal = 0x6E, // KEY_DECIMAL
        Divide = 0x6F, // KEY_DIVIDE
        F1 = 0x70, // KEY_F1
        F2 = 0x71, // KEY_F2
        F3 = 0x72, // KEY_F3
        F4 = 0x73, // KEY_F4
        F5 = 0x74, // KEY_F5
        F6 = 0x75, // KEY_F6
        F7 = 0x76, // KEY_F7
        F8 = 0x77, // KEY_F8
        F9 = 0x78, // KEY_F9
        F10 = 0x79, // KEY_F10
        F11 = 0x7A, // KEY_F11
        F12 = 0x7B, // KEY_F12
        F13 = 0x7C, // KEY_F13
        F14 = 0x7D, // KEY_F14
        F15 = 0x7E, // KEY_F15
        F16 = 0x7F, // KEY_F16
        F17 = 0x80, // KEY_F17
        F18 = 0x81, // KEY_F18
        F19 = 0x82, // KEY_F19
        F20 = 0x83, // KEY_F20
        F21 = 0x84, // KEY_F21
        F22 = 0x85, // KEY_F22
        F23 = 0x86, // KEY_F23
        F24 = 0x87, // KEY_F24
        NumLock = 0x90, // KEY_NUMLOCK
        Scroll = 0x91, // KEY_SCROLL
        NumPadEquals = 0x92, // KEY_NUMPADEQUALS
        LShift = 0xA0, // KEY_LSHIFT
        RShift = 0xA1, // KEY_RSHIFT
        LControl = 0xA2, // KEY_LCONTROL
        RControl = 0xA3, // KEY_RCONTROL
        LMenu = 0xA4, // KEY_LMENU
        RMenu = 0xA5, // KEY_RMENU
        Semicolon = 0xBA, // KEY_SEMICOLON
        Oem1 = Semicolon, // KEY_OEM_1
        Plus = 0xBB, // KEY_PLUS
        Equals = Plus, // KEY_EQUALS
        Comma = 0xBC, // KEY_COMMA
        Minus = 0xBD, // KEY_MINUS
        Period = 0xBE, // KEY_PERIOD
        Slash = 0xBF, // KEY_SLASH
        Oem2 = Slash, // KEY_OEM_2
        Grave = 0xC0, // KEY_GRAVE
        Oem3 = Grave, // KEY_OEM_3
        LBracket = 0xDB, // KEY_LBRACKET
        Oem4 = LBracket, // KEY_OEM_4
        Backslash = 0xDC, // KEY_BACKSLASH
        Oem5 = Backslash, // KEY_OEM_5
        RBracket = 0xDD, // KEY_RBRACKET
        Oem6 = RBracket, // KEY_OEM_6
        Apostrophe = 0xDE, // KEY_APOSTROPHE
        Oem7 = Apostrophe, // KEY_OEM_7
        Oem102 = 0xE2, // KEY_OEM_102
        // KEY_RAGE_EXTRA1 = 0xF0,
        // KEY_RAGE_EXTRA2 = 0xF1,
        // KEY_RAGE_EXTRA3 = 0xF2,
        // KEY_RAGE_EXTRA4 = 0xF4,
        NumPadEnter = 0xFD, // KEY_NUMPADENTER
        // KEY_CHATPAD_GREEN_SHIFT = 0xFE,
        // KEY_CHATPAD_ORANGE_SHIFT = 0xFF,

        MouseAxisX = 0x200, // IOM_AXIS_X
        MouseAxisY = 0x201, // IOM_AXIS_Y
        //IOM_IAXIS_X = 0x202, // same icon as IOM_AXIS_*
        //IOM_IAXIS_Y = 0x203,
        //BASIC_MOUSE_AXIS_MAX = 0x204, // not an icon
        MouseAxisXLeft = 0x205, // IOM_AXIS_X_LEFT
        MouseAxisXRight = 0x206, // IOM_AXIS_X_RIGHT
        MouseAxisYUp = 0x207, // IOM_AXIS_Y_UP
        MouseAxisYDown = 0x208, // IOM_AXIS_Y_DOWN
        //MOUSE_AXIS_MAX = 0x209, // not an icon

        MouseWheelUp = 0x400, // IOM_WHEEL_UP
        MouseWheelDown = 0x401, // IOM_WHEEL_DOWN
        MouseWheel = 0x402, // IOM_AXIS_WHEEL_DELTA
        // these have the same icon as IOM_AXIS_WHEEL_DELTA
        //IOM_IAXIS_WHEEL_DELTA = 0x403,
        //IOM_AXIS_WHEEL = 0x404,
        //IOM_IAXIS_WHEEL = 0x405,
        //IOM_AXIS_WHEEL_RELATIVE = 0x406,
        //IOM_IAXIS_WHEEL_RELATIVE = 0x407,

        MouseLeft = 0x801,
        MouseRight = 0x802,
        MouseMiddle = 0x804,
        MouseExtra1 = 0x808,
        MouseExtra2 = 0x810,
        MouseExtra3 = 0x820,
        MouseExtra4 = 0x840,
        MouseExtra5 = 0x880,

        ControllerAxisLX = 0x1000, // IOM_AXIS_LX
        ControllerAxisLY = 0x1001, // IOM_AXIS_LY
        ControllerAxisRX = 0x1002, // IOM_AXIS_RX
        ControllerAxisRY = 0x1003, // IOM_AXIS_RY
        // IOM_AXIS_LUP = 0x1004, // these don't have icons
        // IOM_AXIS_LDOWN = 0x1005,
        // IOM_AXIS_LLEFT = 0x1006,
        // IOM_AXIS_LRIGHT = 0x1007,
        // IOM_AXIS_LUR = 0x1008,
        // IOM_AXIS_LUL = 0x1009,
        // IOM_AXIS_LDR = 0x100A,
        // IOM_AXIS_LDL = 0x100B,
        // IOM_AXIS_RUP = 0x100C,
        // IOM_AXIS_RDOWN = 0x100D,
        // IOM_AXIS_RLEFT = 0x100E,
        // IOM_AXIS_RRIGHT = 0x100F,
        // IOM_AXIS_RUR = 0x1010,
        // IOM_AXIS_RUL = 0x1011,
        // IOM_AXIS_RDR = 0x1012,
        // IOM_AXIS_RDL = 0x1013,
        // IOM_AXIS_DPADX = 0x1014,
        // IOM_AXIS_DPADY = 0x1015,
        ControllerAxisLYUp = 0x1016, // IOM_AXIS_LY_UP
        ControllerAxisLYDown = 0x1017, // IOM_AXIS_LY_DOWN
        ControllerAxisLXLeft = 0x1018, // IOM_AXIS_LX_LEFT
        ControllerAxisLXRight = 0x1019, // IOM_AXIS_LX_RIGHT
        ControllerAxisRYUp = 0x101A, // IOM_AXIS_RY_UP
        ControllerAxisRYDown = 0x101B, // IOM_AXIS_RY_DOWN
        ControllerAxisRXLeft = 0x101C, // IOM_AXIS_RX_LEFT
        ControllerAxisRXRight = 0x101D, // IOM_AXIS_RX_RIGHT

        ControllerLTrigger = 0x2000, // L2_INDEX
        ControllerRTrigger = 0x2001, // R2_INDEX
        ControllerLShoulder = 0x2002, // L1_INDEX
        ControllerRShoulder = 0x2003, // R1_INDEX
        ControllerRUp = 0x2004, // RUP_INDEX
        ControllerRRight = 0x2005, // RRIGHT_INDEX
        ControllerRDown = 0x2006, // RDOWN_INDEX
        ControllerRLeft = 0x2007, // RLEFT_INDEX
        ControllerSelect = 0x2008, // SELECT_INDEX
        ControllerLThumb = 0x2009, // L3_INDEX
        ControllerLStick = ControllerLThumb,
        ControllerRThumb = 0x200A, // R3_INDEX
        ControllerRStick = ControllerRThumb,
        ControllerStart = 0x200B, // START_INDEX
        ControllerLUp = 0x200C, // LUP_INDEX
        ControllerLRight = 0x200D, // LRIGHT_INDEX
        ControllerLDown = 0x200E, // LDOWN_INDEX
        ControllerLLeft = 0x200F, // LLEFT_INDEX

        // aliases to have the same names as RPH's ControllerButtons enum
        ControllerBack = ControllerSelect,
        ControllerDPadDown = ControllerLDown,
        ControllerDPadRight = ControllerLRight,
        ControllerDPadLeft = ControllerLLeft,
        ControllerDPadUp = ControllerLUp,
        ControllerA = ControllerRDown,
        ControllerB = ControllerRRight,
        ControllerX = ControllerRLeft,
        ControllerY = ControllerRUp,

        // special cases, these don't belong to the original enum
        ControllerDPadNone = 0x40000,
        ControllerDPadAll = 0x40001,
        ControllerDPadUpDown = 0x40002,
        ControllerDPadLeftRight = 0x40003,
        ControllerLThumbRotate = 0x40004,
        ControllerLStickRotate = ControllerLThumbRotate,
        ControllerRThumbRotate = 0x40005,
        ControllerRStickRotate = ControllerRThumbRotate,
        SymbolBusySpinner = 0x40006,
        SymbolPlus = 0x40007,
        SymbolArrowUp = 0x40008,
        SymbolArrowDown = 0x40009,
        SymbolArrowLeft = 0x4000A,
        SymbolArrowRight = 0x4000B,
        SymbolArrowUpDown = 0x4000C,
        SymbolArrowLeftRight = 0x4000D,
        SymbolArrowAll = 0x4000E,
    }
}
