namespace RAGENativeUI.Memory.GFx
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CScaleformStore
    {
        [FieldOffset(0x0038)] public CPool<CScaleformDef> Pool;

        public ref CScaleformDef GetPoolItem(int index)
        {
            return ref Pool[(unchecked((uint)index))];
        }
    }


    [StructLayout(LayoutKind.Explicit, Size = 72)]
    internal unsafe struct ScaleformData1
    {
        [FieldOffset(0x0000)] public short ScaleformIndex; // in ScaleformData2 array
    }

    [StructLayout(LayoutKind.Explicit, Size = 480)]
    internal unsafe struct ScaleformData2
    {
        [FieldOffset(0x00B0)] public int ScaleformStorePoolIndex;
    }
}

