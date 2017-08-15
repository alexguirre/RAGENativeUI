namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTimeCycleModifiersManager
    {
        [FieldOffset(0x0004)] public float Version;

        [FieldOffset(0x0040)] public CTimeCycleModifier.CSimpleArrayPtr Modifiers;
    }
}

