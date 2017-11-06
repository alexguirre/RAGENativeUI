namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;
    
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    internal unsafe struct CTimeCycleModifier
    {
        [FieldOffset(0x0000)] public CArray<Mod> Mods;

        [FieldOffset(0x0010)] public uint Name;
        [FieldOffset(0x0018)] public long unk18;
        [FieldOffset(0x0020)] public uint Flags;
        [FieldOffset(0x0024)] public int unk24;

        public ref Mod GetUnusedModEntry(short increaseCountIfFull = 5)
        {
            if (Mods.Count == Mods.Size)
            {
                short newSize = unchecked((short)(Mods.Size + increaseCountIfFull));
                Mods.Size = newSize;
                Mod* newOffset = (Mod*)GameMemory.Allocator.Allocate(sizeof(Mod) * newSize);
                for (short i = 0; i < Mods.Count; i++)
                {
                    newOffset[i] = Mods[i];
                }
                GameMemory.Allocator.Free(Mods.Offset);
                Mods.Offset = (IntPtr)newOffset;
            }

            short last = Mods.Count;
            Mods.Count++;
            return ref Mods[last];
        }

        public void RemoveModEntry(short index)
        {
            for (short i = index; i < (Mods.Count - 1); i++)
            {
                Mods[i] = Mods[(short)(i + 1)];
            }

            Mods.Count--;
        }

        public void RemoveAllMods()
        {
            int count = Mods.Size * sizeof(Mod);
            for (int i = 0; i < count; i++)
            {
                *(byte*)(Mods.Offset + i) = 0;
            }

            Mods.Count = 0;
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
                    short next = unchecked((short)(sort + 1));
                    if (Mods[sort].ModType > Mods[next].ModType)
                    {
                        Mod temp = Mods[next];
                        Mods[next] = Mods[sort];
                        Mods[sort] = temp;
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

        [StructLayout(LayoutKind.Explicit, Size = 16)]
        public struct SortedEntry
        {
            [FieldOffset(0x0000)] public uint Name;
            [FieldOffset(0x0008)] public IntPtr ModifierPtr;

            public ref CTimeCycleModifier Modifier => ref Unsafe.AsRef<CTimeCycleModifier>(ModifierPtr.ToPointer());
        }
    }
}

