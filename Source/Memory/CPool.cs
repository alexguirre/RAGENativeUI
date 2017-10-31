namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CPool<T> where T : struct
    {
        private long dataAddress;
        private byte* byteArray;
        private uint size;
        private uint itemSize;
        private uint nextEmptyItemSlotIndex; // 0xFFFFFFFF == pool full
        private uint lastInvalidatedItemSlotIndex;
        private uint count;

        public uint Size => size;
        public uint Count => count & 0x3FFFFFFF;
        public uint ItemSize => itemSize;
        public bool IsFull => nextEmptyItemSlotIndex == 0xFFFFFFFF;

        public ref T this[uint index] => ref Get(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid(uint index)
        {
            return index < size && Mask(index) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid(void* item)
        {
            IntPtr address = new IntPtr(item);
            uint i = unchecked((uint)((address.ToInt64() - dataAddress) / itemSize));
            return IsValid(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T Get(uint index)
        {
            return ref Unsafe.AsRef<T>(GetAddress(index).ToPointer());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IntPtr GetAddress(uint index)
        {
            return new IntPtr(Mask(index) & (dataAddress + index * itemSize));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetIndex(void* item)
        {
            IntPtr address = new IntPtr(item);
            uint i = unchecked((uint)((address.ToInt64() - dataAddress) / itemSize));

            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal long Mask(uint i)
        {
            long num1 = byteArray[i] & 0x80;
            return ~((num1 | -num1) >> 63);
        }
    }
}

