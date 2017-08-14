namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 416)]
    internal unsafe struct CPostFX
    {
        [FieldOffset(0x0000)] public uint Name;



        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CSimpleArray
        {
            public CPostFX* Offset;
            public short Count;
            public short Size;

            public CPostFX* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CPostFX)}.{nameof(CSimpleArray)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }
    }
}

