namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    internal unsafe struct fwTxdDef
    {
        [FieldOffset(0x0000)] public IntPtr TexturesDictionaryPtr;

        public ref pgDictionary<grcTexture> TexturesDictionary => ref Unsafe.AsRef<pgDictionary<grcTexture>>(TexturesDictionaryPtr.ToPointer());
    }
}

