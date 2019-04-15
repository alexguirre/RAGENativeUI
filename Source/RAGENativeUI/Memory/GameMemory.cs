namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.CompilerServices;

    using Rage;

    internal static unsafe class GameMemory
    {
        private static void** animPostFXManager;
        public static ref CAnimPostFXManager AnimPostFXManager => ref Unsafe.AsRef<CAnimPostFXManager>(*animPostFXManager);

        private static void* timeCycle;
        public static ref CTimeCycle TimeCycle => ref Unsafe.AsRef<CTimeCycle>(timeCycle);

        internal static bool Init()
        {
            IntPtr address = IntPtr.Zero;

            if (AssertAddress(nameof(CAnimPostFXManager), address = RPH.Game.FindPattern("48 8B 0D ?? ?? ?? ?? C7 44 24 ?? ?? ?? ?? ?? 89 44 24 68 33 C0 89 44 24 30")))
            {
                address = address + *(int*)(address + 3) + 7;
                animPostFXManager = (void**)address;
            }

            if (AssertAddress(nameof(CTimeCycle), address = RPH.Game.FindPattern("48 8D 0D ?? ?? ?? ?? 45 33 C9 89 44 24 38 E8 ?? ?? ?? ?? 83 0D")))
            {
                address = address + *(int*)(address + 3) + 7;
                timeCycle = (void*)address;
            }

            return !anyAssertFailed;
        }

        private static bool anyAssertFailed = false;
        private static bool AssertAddress(string name, IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                Common.Log($"Incompatible game version, couldn't find {name} instance.");
                anyAssertFailed = true;
                return false;
            }

            return true;
        }
    }
}
