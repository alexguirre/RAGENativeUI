namespace RAGENativeUI.Memory.GFx
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 40)]
    internal unsafe struct CScaleformMovieObject
    {
        [FieldOffset(0x18)] private ulong field18;

        public Pointer<GFxMovieRoot> GetMovieRoot()
        {
            ulong tmp = field18;
            if (tmp != 0)
            {
                tmp = *(ulong*)(tmp + 0x10);
                if (tmp != 0)
                {
                    return (void*)tmp;
                }
            }

            return null;
        }
    }
}

