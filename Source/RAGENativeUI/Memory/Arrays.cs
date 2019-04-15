namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal unsafe static class Ref
    {
        public static ref T Null<T>() where T : unmanaged => ref Unsafe.AsRef<T>(null);
        public static bool IsNull<T>(ref T tRef) where T : unmanaged => Unsafe.AreSame(ref Null<T>(), ref tRef);
    }

    [StructLayout(LayoutKind.Sequential, Size = 8)]
    internal unsafe struct Ptr<T> where T : unmanaged
    {
        private readonly T* value;

        public T* Value => value;
        public bool IsNull => value == null;
        public ref T this[int elementOffset] => ref value[elementOffset];

        public Ptr(T* value) => this.value = value;
        public Ptr(void* value) : this((T*)value) { }
        public Ptr(IntPtr value) : this((T*)value) { }
        public Ptr(ref T element) : this(Unsafe.AsPointer(ref element)) { }

        public ref T Deref() => ref Unsafe.AsRef<T>(value);

        public static implicit operator Ptr<T>(T* value) => new Ptr<T>(value);
        public static implicit operator Ptr<T>(void* value) => new Ptr<T>(value);
        public static implicit operator Ptr<T>(IntPtr value) => new Ptr<T>(value);
        public static implicit operator T* (Ptr<T> ptr) => ptr.Value;
        public static implicit operator void* (Ptr<T> ptr) => ptr.Value;
        public static implicit operator IntPtr(Ptr<T> ptr) => (IntPtr)ptr.Value;
    }

    [StructLayout(LayoutKind.Sequential, Size = 16)]
    internal unsafe struct atArray<T> where T : unmanaged
    {
        private Ptr<T> items;
        private ushort count;
        private ushort size;

        public ref T this[int index] => ref items[index];

        public ushort Count
        {
            get => count;
            private set => count = value;
        }

        public ushort Size
        {
            get => size;
            private set => size = value;
        }

        private T* Items
        {
            get => items;
            set => items = value;
        }

        public void Init(ushort size)
        {
            Items = null;
            Count = 0;
            Size = 0;
            EnsureSize(size);
        }

        public ref T Add()
        {
            EnsureSize((ushort)(Count + 1));

            ushort last = Count;
            Count++;
            return ref this[last];
        }

        public void Remove(int index)
        {
            for (int i = index; i < (Count - 1); i++)
            {
                this[i] = this[i + 1];
            }

            Count--;
        }

        public void Clear()
        {
            Count = 0;
        }

        public int IndexOf(ref T item)
        {
            long diff = (long)Unsafe.AsPointer(ref item) - (long)Items;
            int index = unchecked((int)(diff / Unsafe.SizeOf<T>()));
            return (index >= 0 && index < Count) ? index : -1;
        }

        public void Swap(int index1, int index2)
        {
            ref T item1 = ref this[index1];
            ref T item2 = ref this[index2];

            T tmp = item1;
            item1 = item2;
            item2 = tmp;
        }

        private void EnsureSize(ushort minSize)
        {
            const ushort DefaultCapacity = 4;

            if (Size < minSize)
            {
                ushort newSize = (ushort)(Size == 0 ? DefaultCapacity : Size * 2);
                if (newSize < minSize)
                {
                    newSize = minSize;
                }

                T* newBuffer = (T*)RNUI.Helper.Allocate(Unsafe.SizeOf<T>() * newSize);

                if (Count > 0)
                {
                    Unsafe.CopyBlock(newBuffer, Items, (uint)(Unsafe.SizeOf<T>() * Count));
                }

                RNUI.Helper.Free((IntPtr)Items);
                Size = newSize;
                Items = newBuffer;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Size = 24)]
    internal unsafe struct atBinaryMap<TValue, TKey> where TValue : unmanaged
                                                     where TKey : unmanaged, IComparable<TKey>
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct DataPair
        {
            public TKey Key;
            public TValue Value;
        }

        [MarshalAs(UnmanagedType.I1)]
        private bool isSorted;
        private fixed byte pad[7];
        private atArray<DataPair> array;

        public bool IsSorted
        {
            get => isSorted;
            private set => isSorted = value;
        }

        public ref DataPair this[int index] => ref array[index];
        public ushort Count => array.Count;
        public ushort Size => array.Size;

        public ref TValue Find(in TKey key)
        {
            int index = Search(key);

            if (index != -1)
            {
                return ref this[index].Value;
            }
            else
            {
                return ref Ref.Null<TValue>();
            }
        }

        public int Search(in TKey key) => IsSorted ? BinarySearch(key) : LinearSearch(key);

        private int BinarySearch(in TKey key)
        {
            int leftIndex = 0;
            int rightIndex = Count - 1;

            while (leftIndex <= rightIndex)
            {
                int mid = (rightIndex + leftIndex) >> 1;

                ref TKey pairKey = ref this[mid].Key;

                int cmp = pairKey.CompareTo(key);
                if (cmp == 0)
                {
                    return mid;
                }

                if (cmp < 0)
                {
                    leftIndex = mid + 1;
                }
                else
                {
                    rightIndex = mid - 1;
                }
            }

            return -1;
        }

        private int LinearSearch(in TKey key)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Key.CompareTo(key) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Add(in TKey key, in TValue value)
        {
            ref DataPair p = ref array.Add();
            p.Key = key;
            p.Value = value;
            IsSorted = false;
            Sort();
        }

        // This sort is based on TKey.CompareTo(TKey) method, the game may use a different comparison function
        // in some cases but for our case this works.
        public void Sort()
        {
            if (IsSorted)
            {
                return;
            }

            for (ushort write = 0; write < Count; write++)
            {
                for (ushort sort = 0; sort < Count - 1; sort++)
                {
                    ushort next = (ushort)(sort + 1);
                    ref DataPair sortRef = ref this[sort];
                    ref DataPair nextRef = ref this[next];
                    if (sortRef.Key.CompareTo(nextRef.Key) > 0)
                    {
                        // swap
                        DataPair tmp = nextRef;
                        nextRef = sortRef;
                        sortRef = tmp;
                    }
                }
            }
        }
    }

    internal unsafe struct InlinedArray<T> where T : unmanaged
    {
        private T start;

        public ref T this[int index]
        {
            get { fixed (T* a = &start) { return ref a[index]; } }
        }
    }
}