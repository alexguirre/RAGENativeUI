namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit, Size = 760)]
    internal unsafe struct CAnimPostFXManager
    {
        [FieldOffset(0x0000)] public CAnimPostFX.CSimpleArray Effects;

        [FieldOffset(0x0020)] private IntPtr currentActiveEffectPtr;
        [FieldOffset(0x0050)] private IntPtr lastActiveEffectPtr;

        public CAnimPostFX* GetCurrentActiveEffect()
        {
            long v = *(long*)currentActiveEffectPtr;

            if (v == 0 || v == 0x0000800000000200)
                return null;

            CAnimPostFX* p = (CAnimPostFX*)v;
            return p;
        }

        public CAnimPostFX* GetLastActiveEffect()
        {
            CAnimPostFX* p = GetCurrentActiveEffect();
            if(p == null)
                p = *(CAnimPostFX**)lastActiveEffectPtr;
            return p;
        }
    }
}

