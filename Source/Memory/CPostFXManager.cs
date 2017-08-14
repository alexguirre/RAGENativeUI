namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit, Size = 760)]
    internal unsafe struct CPostFXManager
    {
        [FieldOffset(0x0000)] public CPostFX.CSimpleArray Effects;

        [FieldOffset(0x0070)] private IntPtr lastActiveEffectPtr;

        public CPostFX* GetLastActiveEffect()
        {
            CPostFX* e = *(CPostFX**)lastActiveEffectPtr;
            return e;
        }
    }
}

