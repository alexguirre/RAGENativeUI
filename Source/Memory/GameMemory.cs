namespace RAGENativeUI.Memory
{
    using System;

    using Rage;

    internal static unsafe class GameMemory
    {
        public static CAnimPostFXManager** AnimPostFXManager { get; private set; }
        public static CTimeCycle* TimeCycle { get; private set; }
        public static sysMemAllocator* Allocator { get; private set; }

        internal static bool Init()
        {
            IntPtr address = IntPtr.Zero;
            
            if (AssertAddress(nameof(CAnimPostFXManager), address = Game.FindPattern("48 8B 0D ?? ?? ?? ?? C7 44 24 ?? ?? ?? ?? ?? 89 44 24 68 33 C0 89 44 24 30")))
            {
                address = address + *(int*)(address + 3) + 7;
                AnimPostFXManager = (CAnimPostFXManager**)address;
            }
            
            if (AssertAddress(nameof(CTimeCycle), address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 45 33 C9 89 44 24 38 E8 ?? ?? ?? ?? 83 0D")))
            {
                address = address + *(int*)(address + 3) + 7;
                TimeCycle = (CTimeCycle*)address;
            }
            
            if (AssertAddress(nameof(sysMemAllocator), address = Game.FindPattern("48 8D 1D ?? ?? ?? ?? A8 08 75 1D 83 C8 08 48 8B CB 89 05 ?? ?? ?? ??")))
            {
                address = address + *(int*)(address + 3) + 7;
                Allocator = (sysMemAllocator*)address;
            }
            
            return !anyAssertFailed;
        }

        private static bool anyAssertFailed = false;
        private static bool AssertAddress(string name, IntPtr address)
        {
            if (address == IntPtr.Zero)
            {
                Game.LogTrivial($"Incompatible game version, couldn't find {name} instance.");
                anyAssertFailed = true;
                return false;
            }

            return true;
        }
    }
}

