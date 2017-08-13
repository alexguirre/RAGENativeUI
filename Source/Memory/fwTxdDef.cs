namespace RAGENativeUI.Memory
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 24)]
    internal unsafe struct fwTxdDef
    {
        [FieldOffset(0x0000)] public grcTexture.pgDictionary* TexturesDictionary;
    }
}

