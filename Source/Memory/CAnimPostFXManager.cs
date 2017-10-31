namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit, Size = 760)]
    internal unsafe struct CAnimPostFXManager
    {
        [FieldOffset(0x0000)] public CArray<CAnimPostFX> Effects;

        [FieldOffset(0x0020)] private IntPtr currentActiveEffectPtr;
        [FieldOffset(0x0050)] private IntPtr lastActiveEffectPtr;

        public void* GetCurrentActiveEffect()
        {
            long v = *(long*)currentActiveEffectPtr;
            void* activeEffect = null;
            int index = unchecked((int)(v - (long)Effects.Offset) / Unsafe.SizeOf<CAnimPostFX>());

            if(index >= 0 && index < Effects.Count)
            {
                activeEffect = (void*)v;
            }

            return activeEffect;
        }

        public void* GetLastActiveEffect()
        {
            void* activeEffect = GetCurrentActiveEffect();
            if (activeEffect == null)
            {
                long v = *(long*)lastActiveEffectPtr;
                int index = unchecked((int)(v - (long)Effects.Offset) / Unsafe.SizeOf<CAnimPostFX>());

                if (index >= 0 && index < Effects.Count)
                {
                    activeEffect = (void*)v;
                }
            }

            return activeEffect;
        }
    }
}

