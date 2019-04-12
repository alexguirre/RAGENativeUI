namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTimeCycle
    {
        [FieldOffset(0x0004)] public float Version;

        [FieldOffset(0x0008)] public atArray<float> UnkArray;

        [FieldOffset(0x0040)] public atArray<Ptr/* CTimeCycleModifier* */> Modifiers;

        [FieldOffset(0x0050)] public atBinaryMap<Ptr/* CTimeCycleModifier* */, uint> ModifiersMap;


        [FieldOffset(0x27F0)] public int CurrentModifierIndex;
        [FieldOffset(0x27F4)] public float CurrentModifierStrength;
        [FieldOffset(0x27F8)] public int TransitionModifierIndex;
        [FieldOffset(0x27FC)] public float TransitionCurrentStrength; // interpolates from 0.0 to CurrentModifierStrength using TransitionSpeed
        [FieldOffset(0x2800)] public float TransitionSpeed; // CurrentModifierStrength / time

        public bool IsNameUsed(uint name) => ModifiersMap.Search(name) != -1;

        public ref CTimeCycleModifier NewTimeCycleModifier(uint name, CTimeCycleModifier.Mod[] mods, uint flags = 0)
        {
            ref CTimeCycleModifier modifier = ref Unsafe.AsRef<CTimeCycleModifier>(RNUI.Helper.Allocate(Unsafe.SizeOf<CTimeCycleModifier>()).ToPointer());
            modifier.Mods.Init((ushort)mods.Length);
            for (ushort i = 0; i < mods.Length; i++)
            {
                modifier.Mods.Add() = mods[i];
            }
            modifier.SortMods();
            modifier.Name = name;
            modifier.unk18 = 0;
            modifier.Flags = flags;
            modifier.unk24 = -1;

            Ptr modifierPtr = Unsafe.AsPointer(ref modifier);
            Modifiers.Add() = modifierPtr;
            ModifiersMap.Add(name, modifierPtr);
            UnkArray.Add();
                        
            return ref modifier;
        }
    }
}

