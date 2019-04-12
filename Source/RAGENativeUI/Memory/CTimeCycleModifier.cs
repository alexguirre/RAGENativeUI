namespace RAGENativeUI.Memory
{
    using System.Runtime.InteropServices;
    
    [StructLayout(LayoutKind.Explicit, Size = 0x28)]
    internal struct CTimeCycleModifier
    {
        [FieldOffset(0x0000)] public atArray<Mod> Mods;

        [FieldOffset(0x0010)] public uint Name;
        [FieldOffset(0x0018)] public long unk18;
        [FieldOffset(0x0020)] public uint Flags;
        [FieldOffset(0x0024)] public int unk24;

        public void SortMods()
        {
            // https://en.wikipedia.org/wiki/Bubble_sort
            ushort count = Mods.Count;
            for (ushort write = 0; write < count; write++)
            {
                for (ushort sort = 0; sort < count - 1; sort++)
                {
                    ushort next = (ushort)(sort + 1);
                    if (Mods[sort].ModType > Mods[next].ModType)
                    {
                        Mods.Swap(sort, next);
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

