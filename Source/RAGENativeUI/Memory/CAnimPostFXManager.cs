namespace RAGENativeUI.Memory
{
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Explicit, Size = 760)]
    internal unsafe struct CAnimPostFXManager
    {
        [FieldOffset(0x0000)] public atArray<CAnimPostFX> Effects;

        [FieldOffset(0x0020)] private Ptr<Ptr<CAnimPostFX>> currentActiveEffectPtr;
        [FieldOffset(0x0050)] private Ptr<Ptr<CAnimPostFX>> lastActiveEffectPtr;

        public ref CAnimPostFX GetCurrentActiveEffect()
        {
            ref CAnimPostFX current = ref currentActiveEffectPtr.Deref().Deref();
            int index = Effects.IndexOf(ref current);

            if (index >= 0 && index < Effects.Count)
            {
                return ref current;
            }

            return ref Ref.Null<CAnimPostFX>();
        }

        public ref CAnimPostFX GetLastActiveEffect()
        {
            ref CAnimPostFX activeEffect = ref GetCurrentActiveEffect();
            if (Ref.IsNull(ref activeEffect))
            {
                ref CAnimPostFX last = ref lastActiveEffectPtr.Deref().Deref();
                int index = Effects.IndexOf(ref last);

                if (index >= 0 && index < Effects.Count)
                {
                    return ref last;
                }
                else
                {
                    return ref Ref.Null<CAnimPostFX>();
                }
            }

            return ref activeEffect;
        }
    }
}
