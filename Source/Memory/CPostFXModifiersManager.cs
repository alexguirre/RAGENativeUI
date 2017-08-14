namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CPostFXModifiersManager
    {
        [FieldOffset(0x0004)] public float Version;

        [FieldOffset(0x0040)] public CPostFXModifier.CSimpleArrayPtr Modifiers;
    }
}

