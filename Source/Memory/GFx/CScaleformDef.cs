namespace RAGENativeUI.Memory.GFx
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 216)]
    internal unsafe struct CScaleformDef
    {
        [FieldOffset(0x0000)] public Pointer<CScaleformMovieObject> MovieObject;
    }
}

