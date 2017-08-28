namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    internal unsafe struct CTimeCycleModifier
    {
        [FieldOffset(0x0000)] public Mod.CSimpleArray Mods;

        [FieldOffset(0x0010)] public uint Name;
        [FieldOffset(0x0018)] public long unk18;
        [FieldOffset(0x0020)] public uint Flags;
        [FieldOffset(0x0024)] public int unk24;

        public Mod* GetUnusedModEntry(short increaseCountIfFull = 5)
        {
            if (Mods.Count == Mods.Size)
            {
                short newSize = unchecked((short)(Mods.Size + increaseCountIfFull));
                Mods.Size = newSize;
                Mod* newOffset = (Mod*)GameMemory.Allocator->Allocate(sizeof(Mod) * newSize, 16, 0);
                for (short i = 0; i < Mods.Count; i++)
                {
                    newOffset[i] = *Mods.Get(i);
                }
                GameMemory.Allocator->Free((IntPtr)Mods.Offset);
                Mods.Offset = newOffset;
            }

            short last = Mods.Count;
            Mods.Count++;
            return Mods.Get(last);
        }

        public void RemoveModEntry(short index)
        {
            for (short i = index; i < (Mods.Count - 1); i++)
            {
                Mods.Offset[i] = Mods.Offset[i + 1];
            }

            Mods.Count--;
        }

        // call after adding new mods, to maintain the array in the correct order
        public void SortMods()
        {
            // https://en.wikipedia.org/wiki/Bubble_sort
            short count = Mods.Count;
            for (short write = 0; write < count; write++)
            {
                for (short sort = 0; sort < count - 1; sort++)
                {
                    if (Mods.Get(sort)->ModType > Mods.Get(unchecked((short)(sort + 1)))->ModType)
                    {
                        Mod temp = Mods.Offset[unchecked((short)(sort + 1))];
                        Mods.Offset[sort + 1] = Mods.Offset[sort];
                        Mods.Offset[sort] = temp;
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

            [StructLayout(LayoutKind.Sequential)]
            public unsafe struct CSimpleArray
            {
                public Mod* Offset;
                public short Count;
                public short Size;

                public Mod* Get(short index)
                {
                    if (index >= Size)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifier)}.{nameof(Mod)}.{nameof(CSimpleArray)} is {Size}, the index {index} is out of range.");
                    }

                    return &Offset[index];
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CSimpleArrayPtr
        {
            public CTimeCycleModifier** Offset;
            public short Count;
            public short Size;

            public CTimeCycleModifier* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifier)}.{nameof(CSimpleArrayPtr)} is {Size}, the index {index} is out of range.");
                }

                return Offset[index];
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CSortedArray
        {
            [StructLayout(LayoutKind.Explicit, Size = 16)]
            public struct Entry
            {
                [FieldOffset(0x0000)] public uint Name;
                [FieldOffset(0x0008)] public CTimeCycleModifier* Modifier;
            }

            public Entry* Offset;
            public short Count;
            public short Size;

            public Entry* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifier)}.{nameof(CSortedArray)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }
    }
}

