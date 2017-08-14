namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    internal unsafe struct CPostFXModifier
    {
        //[FieldOffset(0x0000)] public ModsArray Mods;

        [FieldOffset(0x0010)] public uint Name;

        [FieldOffset(0x0020)] public uint Flags;


        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CSimpleArrayPtr
        {
            public CPostFXModifier** Offset;
            public short Count;
            public short Size;

            public CPostFXModifier* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CPostFXModifier)}.{nameof(CSimpleArrayPtr)} is {Size}, the index {index} is out of range.");
                }

                return Offset[index];
            }
        }
    }
}

