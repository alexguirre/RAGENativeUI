namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Explicit, Size = 144)]
    internal unsafe struct grcTexture
    {
        [FieldOffset(0x0028)] private IntPtr nameStrPtr;

        [FieldOffset(0x0038)] private IntPtr d3d11Texture;

        [FieldOffset(0x0050)] public ushort width;
        [FieldOffset(0x0052)] public ushort height;
        [FieldOffset(0x0054)] public ushort depth;

        [FieldOffset(0x0058)] public int format;

        [FieldOffset(0x0060)] public IntPtr PreviousTexturePtr;
        [FieldOffset(0x0068)] public IntPtr NextTexturePtr;

        public ref grcTexture PreviousTexture => ref Unsafe.AsRef<grcTexture>(PreviousTexturePtr.ToPointer());
        public ref grcTexture NextTexture => ref Unsafe.AsRef<grcTexture>(NextTexturePtr.ToPointer());

        public string GetName()
        {
            return nameStrPtr == IntPtr.Zero ? null : Marshal.PtrToStringAnsi(nameStrPtr);
        }
    }
}

