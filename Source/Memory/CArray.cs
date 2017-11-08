namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct CArray<T> where T : struct
    {
        public IntPtr Offset;
        public short Count;
        public short Size;
        private uint pad0;

        public ref T this[short index] => ref Unsafe.AsRef<T>((Offset + index * Unsafe.SizeOf<T>()).ToPointer());
    }

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CInlinedArray<T> where T : struct
    {
        private T start;

        public ref T this[int index] => ref Unsafe.AsRef<T>((byte*)Unsafe.AsPointer(ref start) + index * Unsafe.SizeOf<T>());
    }
}

