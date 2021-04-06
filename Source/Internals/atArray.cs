namespace RAGENativeUI.Internals
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    internal unsafe struct atArray<T> where T : unmanaged
    {
        public T* Items;
        public ushort Count;
        public ushort Size;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (index >= 0 && index < Count)
                {
                    return ref Items[index];
                }

                throw new ArgumentOutOfRangeException(nameof(index), index, $"Out of Range (Count:{Count}, Size:{Size})");
            }
        }

        public Enumerator GetEnumerator() => new Enumerator(ref this);

        public ref T Add()
        {
            if (Count == Size)
            {
                var newSize = Size + 16;
                var newByteSize = (ulong)(sizeof(T) * newSize);
                var newItems = sysMemAllocator.TheAllocator.Allocate(newByteSize, 16, 0);
                Buffer.MemoryCopy(Items, newItems, newByteSize, (ulong)(sizeof(T) * Size));
                sysMemAllocator.TheAllocator.Free(Items);
                Items = (T*)newItems;
                Size = (ushort)newSize;
            }

            return ref Items[Count++];
        }

        public T RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Out of Range (Count:{Count}, Size:{Size})");
            }

            var removed = Items[index];
            // move forward all items after the removed one
            for (int i = index; i < (Count - 1); i++)
            {
                Items[i] = Items[i + 1];
            }
            Count--;
            return removed;
        }

        public ref struct Enumerator
        {
            private readonly atArray<T>* array;
            private int index;
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Enumerator(ref atArray<T> arr)
            {
                array = (atArray<T>*)Unsafe.AsPointer(ref arr);
                index = -1;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int newIndex = index + 1;
                if (newIndex < array->Count)
                {
                    index = newIndex;
                    return true;
                }

                return false;
            }

            public ref T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => ref (*array)[index];
            }
        }
    }
}
