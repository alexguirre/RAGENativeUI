namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;
    
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTimeCycleModifiersManager
    {
        [FieldOffset(0x0004)] public float Version;

        [FieldOffset(0x0008)] public CArray<float> UnkArray;

        [FieldOffset(0x0040)] public CPtrsArray<CTimeCycleModifier> Modifiers;

        [FieldOffset(0x0058)] public CArray<CTimeCycleModifier.SortedEntry> SortedModifiers;


        [FieldOffset(0x27F0)] public int CurrentModifierIndex;
        [FieldOffset(0x27F4)] public float CurrentModifierStrength;
        [FieldOffset(0x27F8)] public int TransitionModifierIndex;
        [FieldOffset(0x27FC)] public float TransitionCurrentStrength; // interpolates from 0.0 to CurrentModifierStrength using TransitionSpeed
        [FieldOffset(0x2800)] public float TransitionSpeed; // CurrentModifierStrength / time

        public bool IsNameUsed(uint name)
        {
            int leftIndex = 0;
            int rightIndex = SortedModifiers.Count - 1;

            while (leftIndex <= rightIndex)
            {
                int mid = (rightIndex + leftIndex) >> 1;

                uint hash = SortedModifiers[(short)mid].Name;

                if (hash == name)
                {
                    return true;
                }

                if (name > hash)
                {
                    leftIndex = mid + 1;
                }
                else
                {
                    rightIndex = mid - 1;
                }
            }

            return false;
        }

        public ref CTimeCycleModifier NewTimeCycleModifier(uint name, CTimeCycleModifier.Mod[] mods, uint flags = 0)
        {
            // TODO: should improve NewTimeCycleModifier performance, it takes around 20ms to execute this(depending on how many mods there are, etc.)
            // improving sorting algorithms will probably help
            // not a priority for now
            ref CTimeCycleModifier modifier = ref Unsafe.AsRef<CTimeCycleModifier>(GameMemory.Allocator.Allocate(Unsafe.SizeOf<CTimeCycleModifier>()).ToPointer());
            modifier.Mods.Offset = GameMemory.Allocator.Allocate(Unsafe.SizeOf<CTimeCycleModifier.Mod>() * mods.Length);
            modifier.Mods.Size = (short)mods.Length;
            modifier.Mods.Count = (short)mods.Length;
            for (short i = 0; i < mods.Length; i++)
            {
                modifier.Mods[i] = mods[i];
            }
            modifier.SortMods();
            modifier.Name = name;
            modifier.unk18 = 0;
            modifier.Flags = flags;
            modifier.unk24 = -1;

            *GetModifiersArrayUnusedEntry() = Unsafe.AsPointer(ref modifier);
            ref CTimeCycleModifier.SortedEntry entry = ref GetSortedModifiersArrayUnusedEntry();
            entry.Name = name;
            entry.ModifierPtr = (IntPtr)Unsafe.AsPointer(ref modifier);

            short newUnkArraySize = Modifiers.Count;
            float* newUnkArrayOffset = (float*)GameMemory.Allocator.Allocate(sizeof(float) * newUnkArraySize);
            for (short i = 0; i < UnkArray.Count; i++)
            {
                newUnkArrayOffset[i] = UnkArray[i];
            }
            UnkArray.Count = newUnkArraySize;
            UnkArray.Size = newUnkArraySize;
            GameMemory.Allocator.Free(UnkArray.Offset);
            UnkArray.Offset = (IntPtr)newUnkArrayOffset;

            SortModifiers();
            
            return ref modifier;
        }

        // call after adding new modifier, to maintain the SortedModifiers array in the correct order
        public void SortModifiers()
        {
            // https://en.wikipedia.org/wiki/Bubble_sort
            short count = SortedModifiers.Count;
            for (short write = 0; write < count; write++)
            {
                for (short sort = 0; sort < count - 1; sort++)
                {
                    short next = unchecked((short)(sort + 1));
                    if (SortedModifiers[sort].Name > SortedModifiers[next].Name)
                    {
                        CTimeCycleModifier.SortedEntry temp = SortedModifiers[next];
                        SortedModifiers[next] = SortedModifiers[sort];
                        SortedModifiers[sort] = temp;
                    }
                }
            }
        }


        private void** GetModifiersArrayUnusedEntry(short increaseCountIfFull = 5)
        {
            if (Modifiers.Count == Modifiers.Size)
            {
                short newSize = unchecked((short)(Modifiers.Size + increaseCountIfFull));
                Modifiers.Size = newSize;
                void** newOffset = (void**)GameMemory.Allocator.Allocate(8 * newSize);
                for (short i = 0; i < Modifiers.Count; i++)
                {
                    newOffset[i] = Unsafe.AsPointer(ref Modifiers[i]);
                }
                GameMemory.Allocator.Free(Modifiers.Offset);
                Modifiers.Offset = (IntPtr)newOffset;
            }

            short last = Modifiers.Count;
            Modifiers.Count++;
            return (void**)(Modifiers.Offset + last * 8);
        }

        private ref CTimeCycleModifier.SortedEntry GetSortedModifiersArrayUnusedEntry(short increaseCountIfFull = 5)
        {
            if (SortedModifiers.Count == SortedModifiers.Size)
            {
                short newSize = unchecked((short)(SortedModifiers.Size + increaseCountIfFull));
                SortedModifiers.Size = newSize;
                CTimeCycleModifier.SortedEntry* newOffset = (CTimeCycleModifier.SortedEntry*)GameMemory.Allocator.Allocate(Unsafe.SizeOf<CTimeCycleModifier.SortedEntry>() * newSize);
                for (short i = 0; i < SortedModifiers.Count; i++)
                {
                    newOffset[i] = SortedModifiers[i];
                }
                GameMemory.Allocator.Free(SortedModifiers.Offset);
                SortedModifiers.Offset = (IntPtr)newOffset;
            }

            short last = SortedModifiers.Count;
            SortedModifiers.Count++;
            return ref SortedModifiers[last];
        }
    }
}

