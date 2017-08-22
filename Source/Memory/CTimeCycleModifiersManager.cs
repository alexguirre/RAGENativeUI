namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTimeCycleModifiersManager
    {
        [FieldOffset(0x0004)] public float Version;

        [FieldOffset(0x0008)] public CUnkArray UnkArray;

        [FieldOffset(0x0040)] public CTimeCycleModifier.CSimpleArrayPtr Modifiers;

        [FieldOffset(0x0058)] public CTimeCycleModifier.CSortedArray SortedModifiers;

        public bool IsNameUsed(uint name)
        {
            for (short i = 0; i < SortedModifiers.Count; i++)
            {
                if (SortedModifiers.Get(i)->Name == name)
                    return true;
            }
            return false;
        }

        public CTimeCycleModifier* NewTimeCycleModifier(uint name, CTimeCycleModifier.Mod[] mods, uint flags = 0)
        {
            // TODO: should improve NewTimeCycleModifier performance, it takes around 20ms to execute this(depending on how many mods there are, etc.)
            // improving sorting algorithms will probably help
            // not a priority for now
            CTimeCycleModifier* modifier = (CTimeCycleModifier*)GameMemory.Allocator->Allocate(sizeof(CTimeCycleModifier), 16, 0);
            modifier->Mods.Offset = (CTimeCycleModifier.Mod*)GameMemory.Allocator->Allocate(sizeof(CTimeCycleModifier.Mod) * mods.Length, 16, 0);
            modifier->Mods.Size = (short)mods.Length;
            modifier->Mods.Count = (short)mods.Length;
            for (short i = 0; i < mods.Length; i++)
            {
                modifier->Mods.Offset[i] = mods[i];
            }
            modifier->SortMods();
            modifier->Name = name;
            modifier->unk18 = 0;
            modifier->Flags = flags;
            modifier->unk24 = -1;

            *GetModifiersArrayEntry() = modifier;
            CTimeCycleModifier.CSortedArray.Entry* entry = GetSortedModifiersArrayEntry();
            entry->Name = name;
            entry->Modifier = modifier;

            short newUnkArraySize = Modifiers.Count;
            float* newUnkArrayOffset = (float*)GameMemory.Allocator->Allocate(sizeof(float) * newUnkArraySize, 16, 0);
            for (short i = 0; i < UnkArray.Count; i++)
            {
                newUnkArrayOffset[i] = UnkArray.Offset[i];
            }
            UnkArray.Count = newUnkArraySize;
            UnkArray.Size = newUnkArraySize;
            GameMemory.Allocator->Free((IntPtr)UnkArray.Offset);
            UnkArray.Offset = newUnkArrayOffset;

            SortModifiers();
            
            return modifier;
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
                    if (SortedModifiers.Get(sort)->Name > SortedModifiers.Get(unchecked((short)(sort + 1)))->Name)
                    {
                        CTimeCycleModifier.CSortedArray.Entry temp = SortedModifiers.Offset[unchecked((short)(sort + 1))];
                        SortedModifiers.Offset[sort + 1] = SortedModifiers.Offset[sort];
                        SortedModifiers.Offset[sort] = temp;
                    }
                }
            }
        }


        private CTimeCycleModifier** GetModifiersArrayEntry(short increaseCountIfFull = 1)
        {
            if (Modifiers.Count == Modifiers.Size)
            {
                short newSize = unchecked((short)(Modifiers.Size + increaseCountIfFull));
                Modifiers.Size = newSize;
                CTimeCycleModifier** newOffset = (CTimeCycleModifier**)GameMemory.Allocator->Allocate(8 * newSize, 16, 0);
                for (short i = 0; i < Modifiers.Count; i++)
                {
                    newOffset[i] = Modifiers.Get(i);
                }
                GameMemory.Allocator->Free((IntPtr)Modifiers.Offset);
                Modifiers.Offset = newOffset;
            }

            short last = Modifiers.Count;
            Modifiers.Count++;
            return &Modifiers.Offset[last];
        }

        private CTimeCycleModifier.CSortedArray.Entry* GetSortedModifiersArrayEntry(short increaseCountIfFull = 1)
        {
            if (SortedModifiers.Count == SortedModifiers.Size)
            {
                short newSize = unchecked((short)(SortedModifiers.Size + increaseCountIfFull));
                SortedModifiers.Size = newSize;
                CTimeCycleModifier.CSortedArray.Entry* newOffset = (CTimeCycleModifier.CSortedArray.Entry*)GameMemory.Allocator->Allocate(sizeof(CTimeCycleModifier.CSortedArray.Entry) * newSize, 16, 0);
                for (short i = 0; i < SortedModifiers.Count; i++)
                {
                    newOffset[i] = SortedModifiers.Offset[i];
                }
                GameMemory.Allocator->Free((IntPtr)SortedModifiers.Offset);
                SortedModifiers.Offset = newOffset;
            }

            short last = SortedModifiers.Count;
            SortedModifiers.Count++;
            return &SortedModifiers.Offset[last];
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CUnkArray
        {
            public float* Offset;
            public short Count;
            public short Size;

            public float* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifiersManager)}.{nameof(CUnkArray)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }
    }
}

