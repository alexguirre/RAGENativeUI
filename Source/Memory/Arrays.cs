namespace RAGENativeUI.Memory
{
    using System.Runtime.InteropServices;

    // atArrays
    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct atArray_float
    {
        public float* Items;
        public ushort Count;
        public ushort Size;
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct atArray_CTimeCycleModifierPtr
    {
        public CTimeCycleModifier** Items;
        public ushort Count;
        public ushort Size;
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct atArray_CTimeCycleModifier_Mod
    {
        public CTimeCycleModifier.Mod* Items;
        public ushort Count;
        public ushort Size;
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct atArray_CAnimPostFX
    {
        public CAnimPostFX* Items;
        public ushort Count;
        public ushort Size;
    }

    // atHashSortedArrays
    [StructLayout(LayoutKind.Sequential, Size = 24)]
    internal unsafe struct atHashSortedArray_CTimeCycleModifier
    {
        [StructLayout(LayoutKind.Sequential, Size = 16)]
        public struct Entry
        {
            public uint Hash;
            private uint pad;
            public CTimeCycleModifier* Item;
        }

        [MarshalAs(UnmanagedType.I1)]
        public bool IsSorted;
        private fixed byte pad[7];
        public Entry* Entries;
        public ushort Count;
        public ushort Size;
    }

    // InlinedArrays
    internal unsafe struct InlinedArray_CAnimPostFX_Layer
    {
        private CAnimPostFX.Layer start;

        public CAnimPostFX.Layer* this[int index]
        {
            get { fixed (CAnimPostFX.Layer* a = &start) { return (&a[index]); } }
            set { fixed (CAnimPostFX.Layer* a = &start) { a[index] = *value; } }
        }
    }
}

