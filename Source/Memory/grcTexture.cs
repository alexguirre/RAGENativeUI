namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 144)]
    internal unsafe struct grcTexture
    {
        [FieldOffset(0x0028)] private IntPtr nameStrPtr;

        [FieldOffset(0x0038)] private IntPtr d3d11Texture;

        [FieldOffset(0x0050)] private ushort width;
        [FieldOffset(0x0052)] private ushort height;
        [FieldOffset(0x0054)] private ushort depth;

        [FieldOffset(0x0058)] private int format;

        [FieldOffset(0x0060)] public grcTexture* Previous;
        [FieldOffset(0x0068)] public grcTexture* Next;

        public string GetName()
        {
            return nameStrPtr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(nameStrPtr);
        }

        public ushort GetWidth()
        {
            return width;
        }

        public ushort GetHeight()
        {
            return height;
        }

        public ushort GetDepth()
        {
            return depth;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal unsafe struct CSimpleArrayPtr
        {
            public grcTexture** Offset;
            public ushort Count;
            public ushort Size;

            public grcTexture* Get(ushort index)
            {
                if (index >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"The size of this {nameof(grcTexture)}.{nameof(CSimpleArrayPtr)} is {Size}, the index {index} is out of range.");
                }

                return Offset[index];
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal unsafe struct pgDictionary
        {
            //[FieldOffset(0x0020)] public CSimpleArray_UInt32 Keys;
            [FieldOffset(0x0030)] public CSimpleArrayPtr Values;
        }
    }
}

