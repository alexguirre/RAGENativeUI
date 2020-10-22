namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Rage;

    using Debug = System.Diagnostics.Debug;

    internal unsafe struct TokenParserHookData
    {
        public uint Refs;
        public fixed byte OrigCode[TokenParserHook.OrigCodeLength];
        public IntPtr HookAddr;
        public IntPtr StubAddr;
    }

    internal static unsafe class TokenParserHook
    {
        public const int OrigCodeLength = 8;

        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(TokenParserHook)}] {str}");

        public static void Init()
        {
            ref var data = ref Shared.TokenParserHookData;

            data.Refs++;
            Log($"Init (refs: {data.Refs})");
            if (data.Refs > 1)
            {
                return;
            }

            IntPtr addr = Game.FindPattern("E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? F3 0F 10 45 ?? 0F 2F 45 98");
            if (addr == IntPtr.Zero)
            {
                Log($"addr pattern not found");
                return;
            }

            IntPtr hookAddr = addr + 5; // skip call
            IntPtr failedJumpAddr = hookAddr + *(int*)(hookAddr + 4) + 8;
            IntPtr successJumpAddr = hookAddr + 8;

            var stubCode = Stubs.TokenParser;
            IntPtr stubAddr = AllocateStub(stubCode.Length);

            Log($"hookAddr = {hookAddr.ToString("X16")}");
            Log($"failedJumpAddr = {failedJumpAddr.ToString("X16")}");
            Log($"successJumpAddr = {successJumpAddr.ToString("X16")}");
            Log($"stubAddr = {stubAddr.ToString("X16")}");

            if (stubAddr == IntPtr.Zero)
            {
                Log($"Stub allocation failed");
                return;
            }

            // prepare our stub
            Marshal.Copy(stubCode, 0, stubAddr, stubCode.Length);

            // stub_success
            {
                ulong* ptr = (ulong*)(stubAddr + 8 * 1).ToPointer();
                Debug.Assert(*ptr == 0x1111111111111111, "possibly wrong stub_success offset");
                *ptr = (ulong)successJumpAddr;
                Log("stub_success set");
            }

            // stub_failed
            {
                ulong* ptr = (ulong*)(stubAddr + 8 * 2).ToPointer();
                Debug.Assert(*ptr == 0x2222222222222222, "possibly wrong stub_failed offset");
                *ptr = (ulong)failedJumpAddr;
                Log("stub_failed set");
            }

            // base_address
            {
                ulong* ptr = (ulong*)(stubAddr + 8 * 3).ToPointer();
                Debug.Assert(*ptr == 0x3333333333333333, "possibly wrong base_address offset");
                *ptr = (ulong)stubAddr;
                Log("base_address set");
            }

            // keyboard_layout
            {
                ulong* ptr = (ulong*)(stubAddr + 8 * 4).ToPointer();
                Debug.Assert(*ptr == 0x4444444444444444, "possibly wrong keyboard_layout offset");
                *ptr = (ulong)Memory.CControlMgr_sm_MappingMgr_KeyboardLayout;
                Log("keyboard_layout set");
            }

            var stubOffset = *(int*)stubAddr;
            var stubStartAddr = stubAddr + stubOffset;
            Log($"stubStartAddr = {stubStartAddr.ToString("X16")}");

            // prepare the hook in the original function
            Nop(hookAddr);
            Jmp(hookAddr, stubStartAddr);

            data.StubAddr = stubAddr;
            data.HookAddr = hookAddr;
        }

        public static void Shutdown()
        {
            ref var data = ref Shared.TokenParserHookData;
            data.Refs--;
            Log($"Shutdown (refs: {data.Refs})");
            if (data.Refs != 0)
            {
                return;
            }

            if (data.StubAddr == IntPtr.Zero)
            {
                return;
            }

            RestoreNop();
            VirtualFree(data.StubAddr, 0, MEM_RELEASE);
        }

        private static void Nop(IntPtr addr)
        {
            ref var data = ref Shared.TokenParserHookData;
            data.HookAddr = addr;
            for (int i = 0; i < OrigCodeLength; i++)
            {
                byte* b = (byte*)(addr + i);
                data.OrigCode[i] = *b;
                *b = 0x90;
            }
        }

        private static void RestoreNop()
        {
            ref var data = ref Shared.TokenParserHookData;
            IntPtr addr = data.HookAddr;
            for (int i = 0; i < OrigCodeLength; i++)
            {
                byte* b = (byte*)(addr + i);
                *b = data.OrigCode[i];
            }
        }

        private static void Jmp(IntPtr addr, IntPtr jumpAddr)
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

        private static IntPtr AllocateStub(int length)
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
