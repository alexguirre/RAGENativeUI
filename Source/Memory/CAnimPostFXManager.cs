namespace RAGENativeUI.Memory
{
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Explicit, Size = 760)]
    internal unsafe struct CAnimPostFXManager
    {
        [FieldOffset(0x0000)] public  atArray_CAnimPostFX Effects;

        [FieldOffset(0x0020)] private CAnimPostFX** currentActiveEffectPtr;
        [FieldOffset(0x0050)] private CAnimPostFX** lastActiveEffectPtr;

        public CAnimPostFX* GetCurrentActiveEffect()
        {
            int index = unchecked((int)((long)(*currentActiveEffectPtr) - (long)Effects.Items) / sizeof(CAnimPostFX));

            if(index >= 0 && index < Effects.Count)
            {
                return *currentActiveEffectPtr;
            }
            
            return null;
        }

        public CAnimPostFX* GetLastActiveEffect()
        {
            CAnimPostFX* activeEffect = GetCurrentActiveEffect();
            if (activeEffect == null)
            {
                int index = unchecked((int)((long)(*lastActiveEffectPtr) - (long)Effects.Items) / sizeof(CAnimPostFX));

                if (index >= 0 && index < Effects.Count)
                {
                    return *lastActiveEffectPtr;
                }
                else
                {
                    return null;
                }
            }

            return activeEffect;
        }
    }
}

