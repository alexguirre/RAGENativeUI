namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.CompilerServices;

    using Rage;

    using RAGENativeUI.Memory.GFx;

    internal static unsafe class GameMemory
    {
        private static void* txdStorePtr;
        private static void* animPostFXManagerPtr;
        private static void* scaleformStorePtr;
        private static void* scaleformData1ArrayPtr;
        private static void* scaleformData2ArrayPtr;
        private static void* timeCycleModifiersManagerPtr;
        private static void* allocatorPtr;

        public static ref fwTxdStore TxdStore => ref Unsafe.AsRef<fwTxdStore>(txdStorePtr);
        public static ref CAnimPostFXManager AnimPostFXManager => ref Unsafe.AsRef<CAnimPostFXManager>(animPostFXManagerPtr);
        public static ref CScaleformStore ScaleformStore => ref Unsafe.AsRef<CScaleformStore>(scaleformStorePtr);
        public static ref CInlinedArray<ScaleformData1> ScaleformData1Array => ref Unsafe.AsRef<CInlinedArray<ScaleformData1>>(scaleformData1ArrayPtr);
        public static ref CArray<ScaleformData2> ScaleformData2Array => ref Unsafe.AsRef<CArray<ScaleformData2>>(scaleformData2ArrayPtr);
        public static ref CTimeCycleModifiersManager TimeCycleModifiersManager => ref Unsafe.AsRef<CTimeCycleModifiersManager>(timeCycleModifiersManagerPtr);
        public static ref sysMemAllocator Allocator => ref Unsafe.AsRef<sysMemAllocator>(allocatorPtr);

        internal static bool Init()
        {
            IntPtr address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B 45 EC 4C 8D 45 F0 48 8D 55 EC 48 8D 0D");
            if (AssertAddress(address, nameof(fwTxdStore)))
            {
                address = address + *(int*)(address + 3) + 7;
                txdStorePtr = (void*)address;
            }

            address = Game.FindPattern("48 8B 0D ?? ?? ?? ?? C7 44 24 ?? ?? ?? ?? ?? 89 44 24 68 33 C0 89 44 24 30");
            if (AssertAddress(address, nameof(CAnimPostFXManager)))
            {
                address = address + *(int*)(address + 3) + 7;
                animPostFXManagerPtr = *(void**)address;
            }

            address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 8B D3 E8 ?? ?? ?? ?? 84 C0 74 18");
            if (AssertAddress(address, nameof(CScaleformStore)))
            {
                address = address + *(int*)(address + 3) + 7;
                scaleformStorePtr = (void*)address;
            }

            address = Game.FindPattern("48 8D 35 ?? ?? ?? ?? 48 8D 3C C0 8B 4C FE B8 E8 ?? ?? ?? ?? 84 C0 74 15");
            if (AssertAddress(address, $"{nameof(CInlinedArray<ScaleformData1>)}"))
            {
                address = address + *(int*)(address + 3) + 7;
                scaleformData1ArrayPtr = (void*)address;
            }

            address = Game.FindPattern("48 8B 0D ?? ?? ?? ?? 48 69 D2 ?? ?? ?? ?? 83 BC 0A ?? ?? ?? ?? ?? 0F 94 C0 C3");
            if (AssertAddress(address, $"{nameof(CArray<ScaleformData2>)}"))
            {
                address = address + *(int*)(address + 3) + 7;
                scaleformData2ArrayPtr = (void*)address;
            }

            address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 45 33 C9 89 44 24 38 E8 ?? ?? ?? ?? 83 0D");
            if (AssertAddress(address, nameof(CTimeCycleModifiersManager)))
            {
                address = address + *(int*)(address + 3) + 7;
                timeCycleModifiersManagerPtr = (void*)address;
            }

            address = Game.FindPattern("48 8D 1D ?? ?? ?? ?? A8 08 75 1D 83 C8 08 48 8B CB 89 05 ?? ?? ?? ??");
            if (AssertAddress(address, nameof(sysMemAllocator)))
            {
                address = address + *(int*)(address + 3) + 7;
                allocatorPtr = (void*)address;
            }

            return !anyAssertFailed;
        }

        private static bool anyAssertFailed = false;
        private static bool AssertAddress(IntPtr address, string name)
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

