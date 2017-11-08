namespace RAGENativeUI.Memory
{
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Explicit, Size = 760)]
    internal unsafe struct CAnimPostFXManager
    {
        [FieldOffset(0x0000)] public CArray<CAnimPostFX> Effects;

        [FieldOffset(0x0020)] private Pointer<Pointer<CAnimPostFX>> currentActiveEffectPtr;
        [FieldOffset(0x0050)] private Pointer<Pointer<CAnimPostFX>> lastActiveEffectPtr;

        public Pointer<CAnimPostFX> GetCurrentActiveEffect()
        {
            int index = unchecked((int)((long)currentActiveEffectPtr.Ref.RawPointer - (long)Effects.Offset) / Unsafe.SizeOf<CAnimPostFX>());

            if(index >= 0 && index < Effects.Count)
            {
                return currentActiveEffectPtr.Ref;
            }
            
            return null;
        }

        public Pointer<CAnimPostFX> GetLastActiveEffect()
        {
            Pointer<CAnimPostFX> activeEffect = GetCurrentActiveEffect();
            if (activeEffect.IsNull)
            {
                int index = unchecked((int)((long)lastActiveEffectPtr.Ref.RawPointer - (long)Effects.Offset) / Unsafe.SizeOf<CAnimPostFX>());

                if (index >= 0 && index < Effects.Count)
                {
                    return lastActiveEffectPtr.Ref;
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

