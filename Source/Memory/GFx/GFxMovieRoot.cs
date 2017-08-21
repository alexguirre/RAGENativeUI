namespace RAGENativeUI.Memory.GFx
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct GFxMovieRoot
    {

        [FieldOffset(0x09A0)] public byte BackgroundColorBlue;
        [FieldOffset(0x09A1)] public byte BackgroundColorGreen;
        [FieldOffset(0x09A2)] public byte BackgroundColorRed;
        [FieldOffset(0x09A3)] public byte BackgroundColorAlpha;
    }
}

