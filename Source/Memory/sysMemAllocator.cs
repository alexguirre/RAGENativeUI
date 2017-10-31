namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    
    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct sysMemAllocator
    {
        [FieldOffset(0x0000)] private IntPtr VTable;


        private delegate IntPtr AllocateDelegate(ref sysMemAllocator allocator, long size, long align, int subAllocator);
        private delegate void FreeDelegate(ref sysMemAllocator allocator, IntPtr pointer);

        public IntPtr Allocate(long size) => Allocate(size, 16, 0);
        public IntPtr Allocate(long size, long align, int subAllocator)
        {
            AllocateDelegate fn = Marshal.GetDelegateForFunctionPointer<AllocateDelegate>(new IntPtr(*(long*)(VTable + 0x10)));
            return fn(ref this, size, align, subAllocator);
        }

        public void Free(IntPtr pointer)
        {
            FreeDelegate fn = Marshal.GetDelegateForFunctionPointer<FreeDelegate>(new IntPtr(*(long*)(VTable + 0x20)));
            fn(ref this, pointer);
        }
    }
}

