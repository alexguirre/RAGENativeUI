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
            CAnimPostFX* p = null;
            int index = unchecked((int)(v - (long)Effects.Offset) / sizeof(CAnimPostFX));

            if(index >= 0 && index < Effects.Count)
            {
                p = (CAnimPostFX*)v;
            }

            return p;
        }

        public CAnimPostFX* GetLastActiveEffect()
        {
            CAnimPostFX* p = GetCurrentActiveEffect();
            if (p == null)
            {
                long v = *(long*)lastActiveEffectPtr;
                int index = unchecked((int)(v - (long)Effects.Offset) / sizeof(CAnimPostFX));

                if (index >= 0 && index < Effects.Count)
                {
                    p = (CAnimPostFX*)v;
                }
            }
            return p;
        }
    }
}

