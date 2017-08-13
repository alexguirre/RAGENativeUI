namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 416)]
    internal unsafe struct CScreenEffect
    {
        [FieldOffset(0x0000)] public uint NameHash;



        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CSimpleArray
        {
            public CScreenEffect* Offset;
            public short Count;
            public short Size;

            public CScreenEffect* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CScreenEffect)}.{nameof(CSimpleArray)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }
    }
}

