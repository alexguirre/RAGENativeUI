namespace RAGENativeUI.Internals
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Rage;

    using Debug = System.Diagnostics.Debug;

    internal static unsafe class EmbeddedTexturesHook
    {
        public const int OrigCodeLength = 6;

        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(EmbeddedTexturesHook)}] {str}");

        public static int StubSize => Stubs.EmbeddedTextures.Length;

        public static void Init(IntPtr stubAddr)
        {
            IntPtr addr = Game.FindPattern("48 8B FA 4C 8B C1 48 8D 54 24 ?? 48 8D 0D ?? ?? ?? ?? E8");
            if (addr == IntPtr.Zero)
            {
                Log($"addr pattern not found");
                return;
            }

            IntPtr hookAddr = addr; // skip call
            IntPtr failedJumpAddr = hookAddr + 6;
            IntPtr successJumpAddr = hookAddr + 0x96;

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
            var stubCode = Stubs.EmbeddedTextures;
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

            // fragment_store
            {
                ulong* ptr = (ulong*)(stubAddr + 8 * 3).ToPointer();
                Debug.Assert(*ptr == 0x3333333333333333, "possibly wrong fragment_store offset");
                *ptr = (ulong)Memory.g_FragmentStore;
                Log("fragment_store set");
            }

            // drawable_store
            {
                ulong* ptr = (ulong*)(stubAddr + 8 * 4).ToPointer();
                Debug.Assert(*ptr == 0x4444444444444444, "possibly wrong drawable_store offset");
                *ptr = (ulong)Memory.g_DrawableStore;
                Log("drawable_store set");
            }

            // get_hash_key
            {
                ulong* ptr = (ulong*)(stubAddr + 8 * 5).ToPointer();
                Debug.Assert(*ptr == 0x5555555555555555, "possibly wrong get_hash_key offset");
                *ptr = (ulong)Memory.atStringHash;
                Log("get_hash_key set");
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
            data.EmbeddedTexturesHookAddr = addr;
            for (int i = 0; i < OrigCodeLength; i++)
            {
                byte* b = (byte*)(addr + i);
                data.EmbeddedTexturesHookOrigCode[i] = *b;
                *b = 0x90;
            }
        }

        private static void RestoreNop()
        {
            ref var data = ref Shared.HooksData;
            IntPtr addr = data.EmbeddedTexturesHookAddr;
            for (int i = 0; i < OrigCodeLength; i++)
            {
                byte* b = (byte*)(addr + i);
                *b = data.EmbeddedTexturesHookOrigCode[i];
            }
        }
    }
}
