namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    internal unsafe struct CTimeCycleModifier
    {
        [FieldOffset(0x0000)] public atArray_CTimeCycleModifier_Mod Mods;

        [FieldOffset(0x0010)] public uint Name;
        [FieldOffset(0x0018)] public long unk18;
        [FieldOffset(0x0020)] public uint Flags;
        [FieldOffset(0x0024)] public int unk24;

        public Mod* GetUnusedModEntry(ushort increaseCountIfFull = 5)
        {
            if (Mods.Count == Mods.Size)
            {
                ushort newSize = (ushort)(Mods.Size + increaseCountIfFull);
                Mods.Size = newSize;
                Mod* newOffset = (Mod*)GameMemory.Allocator->Allocate(sizeof(Mod) * newSize);
                for (ushort i = 0; i < Mods.Count; i++)
                {
                    newOffset[i] = Mods.Items[i];
                }
                GameMemory.Allocator->Free((IntPtr)Mods.Items);
                Mods.Items = newOffset;
            }

            ushort last = Mods.Count;
            Mods.Count++;
            return &Mods.Items[last];
        }

        public void RemoveModEntry(ushort index)
        {
            for (ushort i = index; i < (Mods.Count - 1); i++)
            {
                Mods.Items[i] = Mods.Items[(i + 1)];
            }

            Mods.Count--;
        }

        public void RemoveAllMods()
        {
            int count = Mods.Size * sizeof(Mod);
            for (int i = 0; i < count; i++)
            {
                *(byte*)((IntPtr)Mods.Items + i) = 0;
            }

            Mods.Count = 0;
        }

        // call after adding new mods, to maintain the array in the correct order
        public void SortMods()
        {
            // https://en.wikipedia.org/wiki/Bubble_sort
            ushort count = Mods.Count;
            for (ushort write = 0; write < count; write++)
            {
                for (ushort sort = 0; sort < count - 1; sort++)
                {
                    ushort next = (ushort)(sort + 1);
                    if (Mods.Items[sort].ModType > Mods.Items[next].ModType)
                    {
                        Mod temp = Mods.Items[next];
                        Mods.Items[next] = Mods.Items[sort];
                        Mods.Items[sort] = temp;
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Explicit, Size = 12)]
        public struct Mod
        {
            [FieldOffset(0x0000)] public int ModType;
            [FieldOffset(0x0004)] public float Value1;
            [FieldOffset(0x0008)] public float Value2;
        }
    }
}

