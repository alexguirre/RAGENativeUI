namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct pgDictionary<T> where T : struct
    {
        private ulong pad0;
        private ulong pad1;
        private ulong pad2;
        private ulong pad3;
        public CArray<uint> Keys;
        public CPtrsArray<T> Values;
        
        public int FindIndex(uint key)
        {
            int leftIndex = 0;
            int rightIndex = Keys.Count - 1;

            while (leftIndex <= rightIndex)
            {
                int mid = (rightIndex + leftIndex) >> 1;

                uint hash = Keys[(short)mid];

                if(hash == key)
                {
                    return mid;
                }

                if(key > hash)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(uint key) => FindIndex(key) != -1;

        public ref T GetValue(uint key)
        {
            int index = FindIndex(key);
            if (index != -1)
            {
                return ref Values[(short)index];
            }

            throw new InvalidOperationException();
        }
    }
}

