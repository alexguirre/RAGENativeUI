namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CScreenEffectsManager
    {
        [FieldOffset(0x0000)] public CScreenEffect.CSimpleArray Effects;
    }
}

