namespace RAGENativeUI.Internals
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    using Rage;

    using static RAGENativeUI.IL.Unsafe;
    using static RAGENativeUI.IL.Invoker;

    internal static class Memory
    {
        public static readonly IntPtr Screen_GetActualWidth, Screen_GetActualHeight;
        public static readonly IntPtr CTextStyle_ScriptStyle;
        public static readonly IntPtr CScaleformMgr_IsMovieRendering;
        public static readonly IntPtr CScaleformMgr_GetRawMovieView;
        public static readonly IntPtr CBusySpinner_InstructionalButtons;

        static unsafe Memory()
        {
            // TODO: check if patterns are found and provide fallbacks in case they are not found
            const string GetActualResolutionPattern = "48 83 EC 38 0F 29 74 24 ?? 66 0F 6E 35 ?? ?? ?? ?? 48 8D 0D ?? ?? ?? ??";
            Screen_GetActualHeight = Game.FindPattern(GetActualResolutionPattern);
            Screen_GetActualWidth = Game.FindPattern(GetActualResolutionPattern, Screen_GetActualHeight + 1);

            CTextStyle_ScriptStyle = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 44 8A C3 E8 ?? ?? ?? ?? 83 25");
            CTextStyle_ScriptStyle = CTextStyle_ScriptStyle + *(int*)(CTextStyle_ScriptStyle + 3) + 7;

            CScaleformMgr_IsMovieRendering = Game.FindPattern("65 48 8B 04 25 ?? ?? ?? ?? 8B F9 48 8B 04 D0 B9 ?? ?? ?? ?? 8B 14 01") - 0x10;
            CScaleformMgr_GetRawMovieView = Game.FindPattern("0F B7 05 ?? ?? ?? ?? 3B D8 7D 78") - 0x10;

            CBusySpinner_InstructionalButtons = Game.FindPattern("0F B7 05 ?? ?? ?? ?? 33 DB 8B F8 85 C0");
            CBusySpinner_InstructionalButtons = CBusySpinner_InstructionalButtons + *(int*)(CBusySpinner_InstructionalButtons + 3) + 7 - 8;
        }
    }

    internal static class Screen
    {
        /// <summary>
        /// Gets the resolution of the main screen, the one containing the UI in case multiple screens are used.
        /// </summary>
        public static SizeF ActualResolution
        {
            get
            {
                using var tls = UsingTls.Scope();

                return new SizeF(InvokeRetFloat(Memory.Screen_GetActualWidth), InvokeRetFloat(Memory.Screen_GetActualHeight));
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    internal struct CTextStyle
    {
        [FieldOffset(0x00)] public int Color;

        [FieldOffset(0x08)] public float Scale;
        [FieldOffset(0x0C)] public (float Start, float End) Wrap;
        [FieldOffset(0x14)] public int Font;

        [FieldOffset(0x1C)] public byte Justification;

        [FieldOffset(0x1E), MarshalAs(UnmanagedType.I1)] public bool DropShadow;
        [FieldOffset(0x1F), MarshalAs(UnmanagedType.I1)] public bool Outline;


        public static ref CTextStyle ScriptStyle => ref AsRef<CTextStyle>(Memory.CTextStyle_ScriptStyle);
    }

    internal static class CScaleformMgr
    {
        public static unsafe ref GFxMovieView GetRawMovieView(int index) => ref AsRef<GFxMovieView>(InvokeRetPointer(Memory.CScaleformMgr_GetRawMovieView, index));

        public static bool IsMovieRendering(int index)
        {
            // CScaleformMgr::IsMovieRendering checks if the current thread is the render thread,
            // so temporarily set this thread type to the render thread type (2)
            // doesn't seem to cause any issue in this case
            long v = UsingTls.Get(0xB4);
            UsingTls.Set(0xB4, 2);
            bool b = InvokeRetBool(Memory.CScaleformMgr_IsMovieRendering, index);
            UsingTls.Set(0xB4, v);
            return b;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    internal struct GFxValue
    {
        [FieldOffset(0x00)] public IntPtr ObjectInterface;
        [FieldOffset(0x08)] public uint Type;
        [FieldOffset(0x10)] public double Value_NValue;
        [FieldOffset(0x10), MarshalAs(UnmanagedType.I1)] public bool Value_BValue;

        public GFxValue(uint type) : this()
        {
            ObjectInterface = IntPtr.Zero;
            Type = type;
        }

        public new uint GetType() => Type & VTC_TypeMask;
        public double GetNumber() => Value_NValue;

        public const uint VTC_ConvertBit = 0x80;
        public const uint VTC_TypeMask = VTC_ConvertBit | 0x0F;

        public const uint VT_Number = 0x03;

        public const uint VT_ConvertNumber = VTC_ConvertBit | VT_Number;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct GFxMovieView
    {
        [FieldOffset(0x00)] public IntPtr* VTable;

        public bool GetVariable(ref GFxValue val, string pathToVar)
            => InvokeRetBool(VTable[17], AsPointer(ref this), AsPointer(ref val), pathToVar);

        public double GetVariableDouble(string pathToVar)
        {
            GFxValue v = new GFxValue(GFxValue.VT_ConvertNumber);
            GetVariable(ref v, pathToVar);

            if (v.GetType() == GFxValue.VT_Number)
            {
                return v.GetNumber();
            }

            return 0.0;
        }
    }

    internal static class CBusySpinner
    {
        public static ref atArray<int> InstructionalButtons => ref AsRef<atArray<int>>(Memory.CBusySpinner_InstructionalButtons);
    }
}
