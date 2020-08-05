namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Rage;

    using RAGENativeUI.IL;

    internal static unsafe class TokenParserHook
    {
        private static IntPtr hookAddr;
        private static byte[] origCode;
        private static IntPtr failedJumpAddr;
        private static IntPtr successJumpAddr;
        private static IntPtr stubAddr;

        private static ParseTokenDelegate tokenParser;
        private static IntPtr tokenParserAddr;

        private static CControlKeyboardLayoutKey* keyboardLayout;
        private const int KeyboardLayoutSize = 255;

        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(TokenParserHook)}] {str}");

        public static void Init()
        {
            Shared.TokenParserHookRefs++;
            Log($"Init (refs: {Shared.TokenParserHookRefs})");
            if (Shared.TokenParserHookRefs > 1)
            {
                return;
            }

            IntPtr addr = Game.FindPattern("E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? F3 0F 10 45 ?? 0F 2F 45 98");
            if (addr == IntPtr.Zero)
            {
                Log($"addr pattern not found");
                return;
            }

            hookAddr = addr + 5; // skip call
            failedJumpAddr = hookAddr + *(int*)(hookAddr + 4) + 8;
            successJumpAddr = hookAddr + 8;

            IntPtr keyboardLayoutPtr = Game.FindPattern("48 8D 05 ?? ?? ?? ?? 41 B8 ?? ?? ?? ?? 48 C1 E3 04 48 03 D8 44 39 03");
            if (keyboardLayoutPtr == IntPtr.Zero)
            {
                Log($"keyboardLayoutPtr pattern not found");
                return;
            }
            keyboardLayout = (CControlKeyboardLayoutKey*)(keyboardLayoutPtr + *(int*)(keyboardLayoutPtr + 3) + 7);

            stubAddr = AllocateStub();

            // TODO: this will crash when the plugin that owns this delegate gets unloaded but there is still other RNUI instances
            tokenParser = TokenParser;
            tokenParserAddr = Marshal.GetFunctionPointerForDelegate(tokenParser);

            Log($"hookAddr = {hookAddr.ToString("X16")}");
            Log($"failedJumpAddr = {failedJumpAddr.ToString("X16")}");
            Log($"successJumpAddr = {successJumpAddr.ToString("X16")}");
            Log($"stubAddr = {stubAddr.ToString("X16")}");
            Log($"tokenParserAddr = {tokenParserAddr.ToString("X16")}");
            Log($"keyboardLayout = {((IntPtr)keyboardLayout).ToString("X16")}");

            if (stubAddr == IntPtr.Zero)
            {
                Log($"Stub allocation failed");
                return;
            }

            // prepare our stub
            Marshal.Copy(stubCode, 0, stubAddr, stubCode.Length);
            SetJnz(stubAddr + 2, successJumpAddr);
            SetMov(stubAddr + (stubCode.Length - 5 - 6 - 2 - 2 - 10), tokenParserAddr);
            SetJz(stubAddr + (stubCode.Length - 5 - 6), failedJumpAddr);
            Jmp(stubAddr + (stubCode.Length - 5), successJumpAddr);

            // prepare the hook in the original function
            origCode = Nop(hookAddr, 8);
            Jmp(hookAddr, stubAddr);
        }

        public static void Shutdown()
        {
            Shared.TokenParserHookRefs--;
            Log($"Shutdown (refs: {Shared.TokenParserHookRefs})");
            if (Shared.TokenParserHookRefs != 0)
            {
                return;
            }

            if (stubAddr == IntPtr.Zero)
            {
                return;
            }

            Marshal.Copy(origCode, 0, hookAddr, origCode.Length);

            VirtualFree(stubAddr, 0, MEM_RELEASE);
        }

        private static void SetCall(IntPtr callInst, IntPtr funcAddr)
        {
            long offset = funcAddr.ToInt64() - (callInst + 5).ToInt64();

            if (offset < int.MinValue || offset > int.MaxValue)
            {
                Log($"{nameof(SetCall)}: offset does not fit in int32 => {offset.ToString("X16")}");
                return;
            }

            *(int*)(callInst + 1) = (int)offset;
        }

        private static void SetJnz(IntPtr jnzInst, IntPtr jumpAddr) => SetJz(jnzInst, jumpAddr);
        private static void SetJz(IntPtr jzInst, IntPtr jumpAddr)
        {
            long offset = jumpAddr.ToInt64() - (jzInst + 6).ToInt64();

            if (offset < int.MinValue || offset > int.MaxValue)
            {
                Log($"{nameof(SetJz)}: offset does not fit in int32 => {offset.ToString("X16")}");
                return;
            }

            *(int*)(jzInst + 2) = (int)offset;
        }

        // sets value of 'mov rax, value'
        private static void SetMov(IntPtr movInst, IntPtr value)
        {
            *(IntPtr*)(movInst + 2) = value;
        }

        private static byte[] Nop(IntPtr addr, int size)
        {
            byte[] orig = new byte[size];
            for (int i = 0; i< size; i++)
            {
                byte* b = (byte*)(addr + i);
                orig[i] = *b;
                *b = 0x90;
            }
            return orig;
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

        private static IntPtr AllocateStub()
        {
            SYSTEM_INFO info = default;
            GetSystemInfo(ref info);

            const long MaxMemoryRange = 0x40000000;
            const int PageSize = 0x1000;

            if (stubCode.Length > PageSize)
            {
                Log($"AllocateStub: stubCode length greater than allocated page size => {stubCode.Length.ToString("X8")}");
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

        static readonly byte[] stubCode = new byte[]
        {
            0x84, 0xC0,                                                     // test al, al
            0x0F, 0x85, 0x00, 0x00, 0x00, 0x00,                             // jnz  success
            0x48, 0x8D, 0x95, 0x90, 0x01, 0x00, 0x00,                       // lea  rdx,[rbp + 0x190] ; rdx = sIcon*
            0x48, 0x8D, 0x8D, 0x30, 0x03, 0x00, 0x00,                       // lea  rcx,[rbp + 0x330] ; rcx = const char* token
            0x48, 0xB8, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,     // mov  rax, TokenParser
            0xFF, 0xD0,                                                     // call rax
            0x84, 0xC0,                                                     // test al, al
            0x0F, 0x84, 0x00, 0x00, 0x00, 0x00,                             // jz   failed
            0xE9, 0x00, 0x00, 0x00, 0x00,                                   // jmp  success
        };

        private static bool TokenParser(string token, ref sIcon icon)
        {
            Log($"TokenParser({token}, {((IntPtr)Unsafe.AsPointer(ref icon)).ToString("X16")})");

            if (token.Length <= 2)
            {
                Log($" > failed: token too short");
                return false;
            }

            const char Separator = '%';

            int iconIndex = 0;
            int state = 0;
            for (int i = 0; i < token.Length; i++)
            {
                char c = token[i];

                switch (state)
                {
                    case 0: // prefix
                        if (c != 't' && c != 'T' && c != 'b')
                        {
                            Log($" > failed: expected underscore 't', 'T' or 'b', got '{c}'");
                            return false;
                        }

                        icon.iconTypeList[iconIndex] = (byte)c;
                        state = 1;
                        break;
                    case 1: // underscore separator
                        if (c != '_')
                        {
                            Log($" > failed: expected underscore '_', got '{c}'");
                            return false;
                        }
                        state = 2;
                        break;
                    case 2: // button letter or icon
                        if (icon.iconTypeList[iconIndex] == 'b')
                        {
                            // parse the number
                            int end = token.IndexOf(Separator, i);
                            end = end == -1 ? token.Length : end;
                            var numStr = token.Substring(i, end - i);
                            if (!uint.TryParse(numStr, out var num))
                            {
                                Log($" > failed: invalid uint '{numStr}'");
                                return false;
                            }

                            i = end - 1;
                            icon.iconList[iconIndex] = num;
                        }
                        else
                        {
                            //icon.iconList[iconIndex] = (byte)c;

                            byte b = (byte)c;
                            bool found = false;
                            for (int key = 0; key < KeyboardLayoutSize; key++)
                            {
                                if (keyboardLayout[key].Icon <= 1 && keyboardLayout[key].Text[0] == b)
                                {
                                    icon.iconList[iconIndex] = (uint)key;
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                Log($" > failed: no matching key in keyboard layout for '{c}'");
                                return false;
                            }
                        }
                        state = 3;
                        break;
                    case 3: // group separator
                        if (c != Separator)
                        {
                            Log($" > failed: expected group separator '{Separator}', got '{c}'");
                            return false;
                        }
                        iconIndex++;
                        if (iconIndex >= sIcon.MaxIcons)
                        {
                            Log($" > failed: too many icons");
                            return false;
                        }
                        state = 0;
                        break;
                }
            }

            Log($" > success");
            icon.useIdAsMovieName = 0;
            return true;
        }

        [return: MarshalAs(UnmanagedType.I1)]
        private delegate bool ParseTokenDelegate([MarshalAs(UnmanagedType.LPStr)] string token, ref sIcon icon);

        [StructLayout(LayoutKind.Sequential, Size = 0x68)]
        private struct sIcon
        {
            public const int MaxIcons = 4;

            public fixed byte id[64];
            public fixed uint iconList[MaxIcons];
            public fixed byte iconTypeList[MaxIcons];
            public int field_54;
            public int field_58;
            public int field_5C;
            public int field_60;
            public byte useIdAsMovieName;
            public byte field_65;
            public byte field_66;
            public byte field_67;
        }

        [StructLayout(LayoutKind.Sequential, Size = 0x10)]
        private struct CControlKeyboardLayoutKey
        {
            public int Icon;
            public fixed byte Text[10];
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
