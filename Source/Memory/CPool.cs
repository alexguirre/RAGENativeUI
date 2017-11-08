namespace RAGENativeUI.Memory
{
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

        public ref T this[uint index] => ref GetItem(index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid(uint index)
        {
            return index < size && Mask(index) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid(Pointer<T> item)
        {
            uint i = unchecked((uint)(((long)item.RawPointer - dataAddress) / itemSize));
            return IsValid(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid(ref T item)
        {
            uint i = unchecked((uint)(((long)Unsafe.AsPointer(ref item) - dataAddress) / itemSize));
            return IsValid(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref T GetItem(uint index)
        {
            return ref GetItemPointer(index).Ref;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Pointer<T> GetItemPointer(uint index)
        {
            return (void*)(Mask(index) & (dataAddress + index * itemSize));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetIndex(Pointer<T> item)
        {
            uint i = unchecked((uint)(((long)item.RawPointer - dataAddress) / itemSize));
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint GetIndex(ref T item)
        {
            uint i = unchecked((uint)(((long)Unsafe.AsPointer(ref item) - dataAddress) / itemSize));
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

