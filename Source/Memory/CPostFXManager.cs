namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit, Size = 760)]
    internal unsafe struct CPostFXManager
    {
        [FieldOffset(0x0000)] public CPostFX.CSimpleArray Effects;

        [FieldOffset(0x0020)] private IntPtr currentActiveEffectPtr;
        [FieldOffset(0x0050)] private IntPtr lastActiveEffectPtr;

        public CPostFX* GetCurrentActiveEffect()
        {
            long v = *(long*)currentActiveEffectPtr;

            if (v == 0 || v == 0x0000800000000200)
                return null;

            CPostFX* p = (CPostFX*)v;
            return p;
        }

        public CPostFX* GetLastActiveEffect()
        {
            CPostFX* p = GetCurrentActiveEffect();
            if(p == null)
                p = *(CPostFX**)lastActiveEffectPtr;
            return p;
        }
    }
}

