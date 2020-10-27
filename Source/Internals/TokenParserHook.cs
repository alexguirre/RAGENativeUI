namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Rage;

    using Debug = System.Diagnostics.Debug;

    internal static unsafe class TokenParserHook
    {
        public const int OrigCodeLength = 8;

        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(TokenParserHook)}] {str}");

        public static int StubSize => Stubs.TokenParser.Length;

        public static void Init(IntPtr stubAddr)
        {
            IntPtr addr = Game.FindPattern("E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? F3 0F 10 45 ?? 0F 2F 45 98");
            if (addr == IntPtr.Zero)
            {
                Log($"addr pattern not found");
                return;
            }

            IntPtr hookAddr = addr + 5; // skip call
            IntPtr failedJumpAddr = hookAddr + *(int*)(hookAddr + 4) + 8;
            IntPtr successJumpAddr = hookAddr + 8;

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
            var stubCode = Stubs.TokenParser;
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
            Hooks.Jmp(hookAddr, stubStartAddr);
        }

        public static void Shutdown()
        {
            if (Shared.HooksData.EmbeddedTexturesHookAddr == IntPtr.Zero)
            {
                return;
            }

            RestoreNop();
        }

        private static void Nop(IntPtr addr)
        {
            ref var data = ref Shared.HooksData;
            data.TokenParserHookAddr = addr;
            for (int i = 0; i < OrigCodeLength; i++)
            {
                byte* b = (byte*)(addr + i);
                data.TokenParserHookOrigCode[i] = *b;
                *b = 0x90;
            }
        }

        private static void RestoreNop()
        {
            ref var data = ref Shared.HooksData;
            IntPtr addr = data.TokenParserHookAddr;
            for (int i = 0; i < OrigCodeLength; i++)
            {
                byte* b = (byte*)(addr + i);
                *b = data.TokenParserHookOrigCode[i];
            }
        }
    }
}
