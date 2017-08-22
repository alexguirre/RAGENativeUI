namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CTimeCycleModifiersManager
    {
        [FieldOffset(0x0004)] public float Version;

        [FieldOffset(0x0008)] public CUnkArray UnkArray;

        [FieldOffset(0x0040)] public CTimeCycleModifier.CSimpleArrayPtr Modifiers;

        [FieldOffset(0x0058)] public CTimeCycleModifier.CSortedArray SortedModifiers;

        
        [StructLayout(LayoutKind.Sequential)]
        public struct CUnkArray
        {
            public float* Offset;
            public short Count;
            public short Size;

            public float* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(CTimeCycleModifiersManager)}.{nameof(CUnkArray)} is {Size}, the index {index} is out of range.");
                }

                return &Offset[index];
            }
        }
    }
}

