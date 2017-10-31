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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetValueIndex(uint key)
        {
            int index = -1;

            int leftIndex = 0;
            int rightIndex = Keys.Count - 1;

            if (rightIndex < 0)
            {
                index = -1;
            }
            else
            {
                while (true)
                {
                    index = (rightIndex + leftIndex) >> 1;
                    short tmpIndex = (short)((rightIndex + leftIndex) >> 1);
                    if (key == Keys[tmpIndex])
                    {
                        break;
                    }

                    if (key >= Keys[tmpIndex])
                    {
                        leftIndex = index + 1;
                    }
                    else
                    {
                        rightIndex = index - 1;
                    }

                    if (leftIndex > rightIndex)
                    {
                        index = -1;
                    }
                }
            }

            return index;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(uint key) => GetValueIndex(key) != -1;

        public ref T GetValue(uint key)
        {
            int index = GetValueIndex(key);
            if (index != -1)
            {
                return ref Values[(short)index];
            }

            throw new InvalidOperationException();
        }
    }
}

