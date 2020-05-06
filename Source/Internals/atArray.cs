namespace RAGENativeUI.Internals
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using RAGENativeUI.IL;

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
