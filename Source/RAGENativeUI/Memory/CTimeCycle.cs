namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTimeCycle
    {
        [FieldOffset(0x0004)] public float Version;

        [FieldOffset(0x0008)] public atArray_float UnkArray;

        [FieldOffset(0x0040)] public atArray_CTimeCycleModifierPtr Modifiers;

        [FieldOffset(0x0050)] public atHashSortedArray_CTimeCycleModifier SortedModifiers;


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

                uint hash = SortedModifiers.Entries[mid].Hash;

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

        public CTimeCycleModifier* NewTimeCycleModifier(uint name, CTimeCycleModifier.Mod[] mods, uint flags = 0)
        {
            // TODO: should improve NewTimeCycleModifier performance, it takes around 20ms to execute this(depending on how many mods there are, etc.)
            // improving sorting algorithms will probably help
            // not a priority for now
            CTimeCycleModifier* modifier = (CTimeCycleModifier*)RNUI.Helper.Allocate(sizeof(CTimeCycleModifier)).ToPointer();
            modifier->Mods.Items = (CTimeCycleModifier.Mod*)RNUI.Helper.Allocate(sizeof(CTimeCycleModifier.Mod) * mods.Length);
            modifier->Mods.Size = (ushort)mods.Length;
            modifier->Mods.Count = (ushort)mods.Length;
            for (ushort i = 0; i < mods.Length; i++)
            {
                modifier->Mods.Items[i] = mods[i];
            }
            modifier->SortMods();
            modifier->Name = name;
            modifier->unk18 = 0;
            modifier->Flags = flags;
            modifier->unk24 = -1;

            *GetModifiersArrayUnusedEntry() = modifier;
            atHashSortedArray_CTimeCycleModifier.Entry* entry = GetSortedModifiersArrayUnusedEntry();
            entry->Hash = name;
            entry->Item = modifier;

            ushort newUnkArraySize = Modifiers.Count;
            float* newUnkArrayOffset = (float*)RNUI.Helper.Allocate(sizeof(float) * newUnkArraySize);
            for (short i = 0; i < UnkArray.Count; i++)
            {
                newUnkArrayOffset[i] = UnkArray.Items[i];
            }
            UnkArray.Count = newUnkArraySize;
            UnkArray.Size = newUnkArraySize;
            RNUI.Helper.Free((IntPtr)UnkArray.Items);
            UnkArray.Items = newUnkArrayOffset;

            SortModifiers();
            
            return modifier;
        }

        // call after adding new modifier, to maintain the SortedModifiers array in the correct order
        public void SortModifiers()
        {
            // https://en.wikipedia.org/wiki/Bubble_sort
            ushort count = SortedModifiers.Count;
            for (ushort write = 0; write < count; write++)
            {
                for (ushort sort = 0; sort < count - 1; sort++)
                {
                    ushort next = (ushort)(sort + 1);
                    if (SortedModifiers.Entries[sort].Hash > SortedModifiers.Entries[next].Hash)
                    {
                        atHashSortedArray_CTimeCycleModifier.Entry* temp = &SortedModifiers.Entries[next];
                        SortedModifiers.Entries[next] = SortedModifiers.Entries[sort];
                        SortedModifiers.Entries[sort] = *temp;
                    }
                }
            }
        }


        private CTimeCycleModifier** GetModifiersArrayUnusedEntry(short increaseCountIfFull = 5)
        {
            if (Modifiers.Count == Modifiers.Size)
            {
                ushort newSize = (ushort)(Modifiers.Size + increaseCountIfFull);
                Modifiers.Size = newSize;
                CTimeCycleModifier** newItems = (CTimeCycleModifier**)RNUI.Helper.Allocate(8 * newSize);
                for (short i = 0; i < Modifiers.Count; i++)
                {
                    newItems[i] = Modifiers.Items[i];
                }
                RNUI.Helper.Free((IntPtr)Modifiers.Items);
                Modifiers.Items = newItems;
            }

            ushort last = Modifiers.Count;
            Modifiers.Count++;
            return &Modifiers.Items[last];
        }

        private atHashSortedArray_CTimeCycleModifier.Entry* GetSortedModifiersArrayUnusedEntry(short increaseCountIfFull = 5)
        {
            if (SortedModifiers.Count == SortedModifiers.Size)
            {
                ushort newSize = (ushort)(SortedModifiers.Size + increaseCountIfFull);
                SortedModifiers.Size = newSize;
                atHashSortedArray_CTimeCycleModifier.Entry* newEntries = (atHashSortedArray_CTimeCycleModifier.Entry*)RNUI.Helper.Allocate(sizeof(atHashSortedArray_CTimeCycleModifier.Entry) * newSize);
                for (short i = 0; i < SortedModifiers.Count; i++)
                {
                    newEntries[i] = SortedModifiers.Entries[i];
                }
                RNUI.Helper.Free((IntPtr)SortedModifiers.Entries);
                SortedModifiers.Entries = newEntries;
            }

            ushort last = SortedModifiers.Count;
            SortedModifiers.Count++;
            return &SortedModifiers.Entries[last];
        }
    }
}

