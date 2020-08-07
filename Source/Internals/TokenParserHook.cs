namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;

    using Rage;

    using RAGENativeUI.IL;

    internal unsafe struct TokenParserHookData
    {
        public uint Refs;
        public fixed byte OrigCode[TokenParserHook.OrigCodeLength];
        public IntPtr HookAddr;
        public IntPtr StubAddr;
        public TokenParserEntryList Parsers;
    }

    internal struct TokenParserEntry
    {
        public int Id;
        public bool Active;
        public IntPtr Addr; // function pointer of the TokenParser of a plugin

        public TokenParserEntry(int id, IntPtr parserAddr)
            => (Id, Active, Addr) = (id, false, parserAddr);
    }

    internal unsafe struct TokenParserEntryList
    {
        public TokenParserEntry* Items;
        public int Count;
        public int Capacity;

        public void Free()
        {
            if (Items != null)
            {
                Marshal.FreeHGlobal((IntPtr)Items);
            }
            Count = 0;
            Capacity = 0;
        }

        public void Add(int id, IntPtr parserAddr)
        {
            EnsureCapacity(Count + 1);
            Items[Count] = new TokenParserEntry(id, parserAddr);
            Count++;
        }

        public int IndexOf(int id)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Items[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Remove(int id)
        {
            int i = IndexOf(id);
            if (i == -1)
            {
                return;
            }

            Items[i] = Items[Count - 1];
            Count--;
        }

        public bool IsActive(int id)
        {
            int i = IndexOf(id);
            if (i == -1)
            {
                return false;
            }

            return Items[i].Active;
        }

        private void EnsureCapacity(int minCapacity)
        {
            if (Capacity >= minCapacity)
            {
                return;
            }

            int newCapacity = Math.Max(Capacity + 16, minCapacity);

            TokenParserEntry* newItems = (TokenParserEntry*)Marshal.AllocHGlobal(newCapacity * sizeof(TokenParserEntry));
            if (Items != null)
            {
                Buffer.MemoryCopy(Items, newItems, newCapacity * sizeof(TokenParserEntry), Count * sizeof(TokenParserEntry));
                Marshal.FreeHGlobal((IntPtr)Items);
            }
            Items = newItems;
        }
    }

    internal static unsafe class TokenParserHook
    {
        public const int OrigCodeLength = 8;

        private static ParseTokenDelegate tokenParser;
        private static IntPtr tokenParserAddr;

        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(TokenParserHook)}] {str}");

        public static void Init()
        {
            ref var data = ref Shared.TokenParserHookData;

            tokenParser = TokenParser;
            tokenParserAddr = Marshal.GetFunctionPointerForDelegate(tokenParser);
            Log($"tokenParserAddr = {tokenParserAddr.ToString("X16")}");

            data.Parsers.Add(AppDomain.CurrentDomain.Id, tokenParserAddr);

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

            IntPtr stubAddr = AllocateStub();

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
            Marshal.Copy(StubCode, 0, stubAddr, StubCode.Length);
            SetJnz(stubAddr + 2, successJumpAddr);
            SetTokenParser(stubAddr);
            SetJz(stubAddr + (StubCode.Length - 5 - 6), failedJumpAddr);
            Jmp(stubAddr + (StubCode.Length - 5), successJumpAddr);

            // prepare the hook in the original function
            Nop(hookAddr);
            Jmp(hookAddr, stubAddr);

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
                int id = AppDomain.CurrentDomain.Id;

                bool needsNewParser = data.Parsers.IsActive(id);
                data.Parsers.Remove(id);
                if (needsNewParser)
                {
                    // change the token parser function pointer if the current plugin owns the active parser
                    SetTokenParser(data.StubAddr);
                }

                return;
            }

            data.Parsers.Free();
            if (data.StubAddr == IntPtr.Zero)
            {
                return;
            }

            RestoreNop();
            VirtualFree(data.StubAddr, 0, MEM_RELEASE);
        }

        // sets the current token parser function pointer to the first entry in the parsers list
        private static void SetTokenParser(IntPtr stubAddr)
        {
            ref var data = ref Shared.TokenParserHookData;
            ref var parser = ref data.Parsers.Items[0];
            parser.Active = true;
            SetMov(stubAddr + (StubCode.Length - 5 - 6 - 2 - 2 - 10), parser.Addr);
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

        private static IntPtr AllocateStub()
        {
            SYSTEM_INFO info = default;
            GetSystemInfo(ref info);

            const long MaxMemoryRange = 0x40000000;
            const int PageSize = 0x1000;

            if (StubCode.Length > PageSize)
            {
                Log($"AllocateStub: stubCode length greater than allocated page size => {StubCode.Length.ToString("X8")}");
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

        static readonly byte[] StubCode = new byte[]
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

        private static bool TokenParser(byte* token, ref sIcon icon)
        {
            int tokenLength = StrLen(token);
            Log($"TokenParser({Encoding.UTF8.GetString(token, tokenLength)}, {((IntPtr)Unsafe.AsPointer(ref icon)).ToString("X16")})");

            if (tokenLength == 1 && token[0] == '+')
            {
                // alias for 'b_998' (plus symbol)
                icon.iconTypeList[0] = (byte)'b';
                icon.iconList[0] = 998;
                icon.useIdAsMovieName = 0;
                Log($" > success: '+' alias");
                return true;
            }

            if (tokenLength <= 2)
            {
                Log($" > failed: token too short");
                return false;
            }

            const char Separator = '%';

            int iconIndex = 0;
            int state = 0;
            for (int i = 0; i < tokenLength; i++)
            {
                byte c = token[i];

                switch (state)
                {
                    case 0: // prefix
                        if (c != 't' && c != 'T' && c != 'b')
                        {
                            Log($" > failed: expected 't', 'T' or 'b', got '{(char)c}'");
                            return false;
                        }

                        icon.iconTypeList[iconIndex] = c;
                        state = 1;
                        break;
                    case 1: // underscore separator
                        if (c != '_')
                        {
                            Log($" > failed: expected underscore '_', got '{(char)c}'");
                            return false;
                        }
                        state = 2;
                        break;
                    case 2: // button letter or icon
                        if (icon.iconTypeList[iconIndex] == 'b')
                        {
                            // parse the number
                            string tokenStr = Encoding.UTF8.GetString(token, tokenLength); // TODO: don't allocate a new string here
                            int end = tokenStr.IndexOf(Separator, i);
                            end = end == -1 ? tokenLength : end;
                            var numStr = tokenStr.Substring(i, end - i);
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
                            if (CControlKeyboardLayoutKey.Available)
                            {
                                bool found = false;
                                for (int key = 0; key < CControlKeyboardLayoutKey.KeyboardLayoutSize; key++)
                                {
                                    ref var layoutKey = ref CControlKeyboardLayoutKey.KeyboardLayout[key];
                                    if (layoutKey.Icon <= 1)
                                    {
                                        bool match = true;

                                        int len = 0;
                                        for (; len < CControlKeyboardLayoutKey.MaxTextLength && layoutKey.Text[len] != 0; len++)
                                        {
                                            if (layoutKey.Text[len] != token[i + len])
                                            {
                                                match = false;
                                                break;
                                            }
                                        }

                                        if (match)
                                        {
                                            i += len - 1;
                                            icon.iconList[iconIndex] = (uint)key;
                                            found = true;
                                            break;
                                        }
                                    }
                                }

                                if (!found)
                                {
                                    Log($" > failed: no matching key in keyboard layout for '{(char)c}'");
                                    return false;
                                }
                            }
                            else
                            {
                                // fallback, only valid for alphanumeric keys
                                icon.iconList[iconIndex] = c;
                            }
                        }
                        state = 3;
                        break;
                    case 3: // group separator
                        if (c != Separator)
                        {
                            Log($" > failed: expected group separator '{Separator}', got '{(char)c}'");
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

            static int StrLen(byte* str)
            {
                int len = 0;
                while (str[len] != 0) { len++; }
                return len;
            }
        }

        [return: MarshalAs(UnmanagedType.I1)]
        private delegate bool ParseTokenDelegate(byte* token, ref sIcon icon);

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
