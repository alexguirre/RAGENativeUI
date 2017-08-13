namespace RAGENativeUI.Memory.GFx
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CScaleformStore
    {
        [FieldOffset(0x0038)] public CPool Pool;

        public CScaleformDef* GetPoolItem(int index)
        {
            return (CScaleformDef*)Pool.Get(unchecked((uint)index));
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 72)]
    internal unsafe struct ScaleformData1
    {
        [FieldOffset(0x0000)] public short ScaleformIndex; // in ScaleformData2 array


        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CArray
        {
            private ScaleformData1 start;

            public ScaleformData1* Get(int index)
            {
                fixed (ScaleformData1* array = &start)
                {
                    return &array[index];
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 480)]
    internal unsafe struct ScaleformData2
    {
        [FieldOffset(0x00B0)] public int ScaleformStorePoolIndex;


        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct CSimpleArray
        {
            public ScaleformData2* Offset;
            public short Count;
            public short Size;

            public ScaleformData2* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(ScaleformData2)}.{nameof(CSimpleArray)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }
    }
}

