namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;

    using Rage;

    internal unsafe struct HooksData
    {
        public uint Refs;
        public IntPtr StubAddr;
        public fixed byte EmbeddedTexturesHookOrigCode[EmbeddedTexturesHook.OrigCodeLength];
        public IntPtr EmbeddedTexturesHookAddr;
        public fixed byte TokenParserHookOrigCode[TokenParserHook.OrigCodeLength];
        public IntPtr TokenParserHookAddr;
    }

    internal static class Hooks
    {
        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(Hooks)}] {str}");

        public static void Init()
        {
            ref var data = ref Shared.HooksData;

            data.Refs++;
            Log($"Init (refs: {data.Refs})");
            if (data.Refs > 1)
            {
                return;
            }

            var embeddedTexturesStubSize = Align(EmbeddedTexturesHook.StubSize);
            var tokenParserStubSize = Align(TokenParserHook.StubSize);

            var stubAddr = AllocateStub(embeddedTexturesStubSize + tokenParserStubSize);

            EmbeddedTexturesHook.Init(stubAddr);
            TokenParserHook.Init(stubAddr + embeddedTexturesStubSize);

            static int Align(int value)
            {
                const int Alignment = 0x10;

                int r = value % Alignment;
                return r != 0 ? value + (Alignment - r) : value;
            }
        }

        public static void Shutdown()
        {
            ref var data = ref Shared.HooksData;
            data.Refs--;
            Log($"Shutdown (refs: {data.Refs})");
            if (data.Refs != 0)
            {
                return;
            }

            TokenParserHook.Shutdown();
            EmbeddedTexturesHook.Shutdown();

            if (data.StubAddr != IntPtr.Zero)
            {
                VirtualFree(data.StubAddr, 0, MEM_RELEASE);
            }
        }

        internal static unsafe void Jmp(IntPtr addr, IntPtr jumpAddr)
        {
            long offset = jumpAddr.ToInt64() - (addr + 5).ToInt64();

            if (offset < int.MinValue || offset > int.MaxValue)
            {
                Log($"{nameof(Jmp)}: offset does not fit in int32 => {offset.ToString("X16")}");
                return;
            }

            *(byte*)(addr + 0) = 0xE9;
            *(int*)(addr + 1) = (int)offset;
        }

        private static unsafe IntPtr AllocateStub(int length)
        {
            SYSTEM_INFO info = default;
            GetSystemInfo(ref info);

            const long MaxMemoryRange = 0x40000000;
            const int PageSize = 0x1000;

            if (length > PageSize)
            {
                Log($"AllocateStub: stubCode length greater than allocated page size => {length.ToString("X8")}");
                return IntPtr.Zero;
            }

            long addr = Process.GetCurrentProcess().MainModule.BaseAddress.ToInt64();
            long minAddr = addr - MaxMemoryRange;

            Log($"AllocateStub: stubSearch StartAddr = {addr.ToString("X16")}");
            Log($"AllocateStub: stubSearch MinAddr = {minAddr.ToString("X16")}");
            Log($"AllocateStub: stubSearch AllocGranularity = {info.dwAllocationGranularity.ToString("X8")}");

            addr -= addr % info.dwAllocationGranularity;
            addr -= info.dwAllocationGranularity;
            while (addr >= minAddr)
            {
                MEMORY_BASIC_INFORMATION memInfo = default;
                if (VirtualQuery((IntPtr)addr, ref memInfo, sizeof(MEMORY_BASIC_INFORMATION)) == 0)
                {
                    break;
                }

                if (memInfo.State == MEM_FREE)
                {
                    Log($"AllocateStub: stubSearch Found free memory = {addr.ToString("X16")}");
                    IntPtr allocAddr = VirtualAlloc((IntPtr)addr, PageSize, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);

                    if (allocAddr != IntPtr.Zero)
                    {
                        return allocAddr;
                    }
                }

                if (memInfo.AllocationBase.ToUInt64() < info.dwAllocationGranularity)
                {
                    break;
                }

                addr = (long)(memInfo.AllocationBase.ToUInt64() - info.dwAllocationGranularity);
            }

            return IntPtr.Zero;
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFree(IntPtr lpAddress, int dwSize, uint dwFreeType);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern int VirtualQuery(IntPtr lpAddress, ref MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern void GetSystemInfo(ref SYSTEM_INFO lpSystemInfo);

        const uint MEM_COMMIT = 0x1000, MEM_RESERVE = 0x2000;
        const uint MEM_RELEASE = 0x4000;
        const uint PAGE_EXECUTE_READWRITE = 0x40;
        const uint MEM_FREE = 0x00010000;

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORY_BASIC_INFORMATION
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            internal PROCESSOR_INFO_UNION p;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public UIntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)] internal uint dwOemId;
            [FieldOffset(0)] internal ushort wProcessorArchitecture;
            [FieldOffset(2)] internal ushort wReserved;
        }
    }
}
