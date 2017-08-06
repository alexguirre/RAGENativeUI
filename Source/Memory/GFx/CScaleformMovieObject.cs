namespace RAGENativeUI.Memory.GFx
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Explicit, Size = 40)]
    internal unsafe struct CScaleformMovieObject
    {


        public GFxMovieRoot* GetMovieRoot()
        {
            fixed (CScaleformMovieObject* thisPtr = &this)
            {
                ulong tmp = (ulong)thisPtr;
                tmp = *(ulong*)(tmp + 0x18);
                if (tmp != 0)
                {
                    tmp = *(ulong*)(tmp + 0x10);
                    if (tmp != 0)
                    {
                        return (GFxMovieRoot*)tmp;
                    }
                }
                return null;
            }
        }
    }
}

