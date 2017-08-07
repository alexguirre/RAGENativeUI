namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct CPool
    {
        [FieldOffset(0x00)] private long dataAddress;
        [FieldOffset(0x08)] private byte* byteArray;
        [FieldOffset(0x10)] private uint size;
        [FieldOffset(0x14)] private uint itemSize;
        [FieldOffset(0x18)] private uint nextEmptyItemSlotIndex; // 0xFFFFFFFF == pool full
        [FieldOffset(0x1C)] private uint lastInvalidatedItemSlotIndex;
        [FieldOffset(0x20)] private uint count;
        [FieldOffset(0x24)] private uint unk;

        public uint Size => size;
        public uint Count => count & 0x3FFFFFFF;
        public uint ItemSize => itemSize;
        public bool IsFull => nextEmptyItemSlotIndex == 0xFFFFFFFF;

        public bool IsValid(uint index)
        {
            return index < size && Mask(index) != 0;
        }

        public bool IsValid(void* item)
        {
            IntPtr address = new IntPtr(item);
            uint i = unchecked((uint)((address.ToInt64() - dataAddress) / itemSize));
            return IsValid(i);
        }

        public void* Get(uint index)
        {
            return (void*)GetAddress(index);
        }

        public IntPtr GetAddress(uint index)
        {
            return new IntPtr(Mask(index) & (dataAddress + index * itemSize));
        }

        public uint GetIndex(void* item)
        {
            IntPtr address = new IntPtr(item);
            uint i = unchecked((uint)((address.ToInt64() - dataAddress) / itemSize));

            if (i >= size)
            {
                throw new InvalidOperationException($"The passed pointer ({address.ToString("X")}) isn't contained in the pool.");
            }

            return i;
        }


        internal long Mask(uint i)
        {
            long num1 = byteArray[i] & 0x80;
            return ~((num1 | -num1) >> 63);
        }
    }
}

