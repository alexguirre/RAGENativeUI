namespace RAGENativeUI.Internals
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    internal static class Variables
    {
        private static readonly IntPtr scriptTextStyle;

        static unsafe Variables()
        {
            // TODO: move this to Module Initializer
            IntPtr address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 44 8A C3 E8 ?? ?? ?? ?? 83 25");

            scriptTextStyle = address + *(int*)(address + 3) + 7;
        }

        public static ref CTextStyle ScriptTextStyle => ref IL.Unsafe.AsRef<CTextStyle>(scriptTextStyle);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    internal struct CTextStyle
    {
        [FieldOffset(0x00)] public int Color;
        
        [FieldOffset(0x08)] public float Scale;
        [FieldOffset(0x0C)] public (float Start, float End) Wrap;
        [FieldOffset(0x14)] public int Font;
        
        [FieldOffset(0x1C)] public byte Justification;
        
        [FieldOffset(0x1E), MarshalAs(UnmanagedType.I8)] public bool DropShadow;
        [FieldOffset(0x1F), MarshalAs(UnmanagedType.I8)] public bool Outline;
    }
}
