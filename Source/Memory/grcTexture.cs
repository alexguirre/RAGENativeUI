namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 144)]
    internal unsafe struct grcTexture
    {
        [FieldOffset(0x0028)] private IntPtr nameStrPtr;

        [FieldOffset(0x0038)] private IntPtr d3d11Texture;

        [FieldOffset(0x0050)] public ushort width;
        [FieldOffset(0x0052)] public ushort height;
        [FieldOffset(0x0054)] public ushort depth;

        [FieldOffset(0x0058)] public int format;

        [FieldOffset(0x0060)] public grcTexture* Previous;
        [FieldOffset(0x0068)] public grcTexture* Next;

        public string GetName()
        {
            return nameStrPtr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(nameStrPtr);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct CRefsArray
        {
            public grcTexture** Offset;
            public short Count;
            public short Size;

            public grcTexture* Get(short index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(grcTexture)}.{nameof(CRefsArray)} is {Size}, the index {index} is out of range.");
                }

                return Offset[index];
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct pgDictionary
        {
            [FieldOffset(0x0020)] public CKeysArray Keys;
            [FieldOffset(0x0030)] public CRefsArray Values;

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
                        if (key == Keys.Get(tmpIndex))
                        {
                            break;
                        }

                        if (key >= Keys.Get(tmpIndex))
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

            public grcTexture* GetValue(uint key)
            {
                int index = GetValueIndex(key);
                if(index != -1)
                {
                    return Values.Get((short)index);
                }

                return null;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct CKeysArray
            {
                public uint* Offset;
                public short Count;
                public short Size;

                public uint Get(short index)
                {
                    if (index >= Size)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(grcTexture)}.{nameof(pgDictionary)}.{nameof(CKeysArray)} is {Size}, the index {index} is out of range.");
                    }

                    return Offset[index];
                }
            }
        }
    }
}

