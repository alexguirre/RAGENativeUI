namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    using RAGENativeUI.Memory.GFx;

    internal static unsafe class GameMemory
    {
        public static fwTxdStore* TxdStore { get; private set; }
        public static CAnimPostFXManager* AnimPostFXManager { get; private set; }
        public static CScaleformStore* ScaleformStore { get; private set; }
        public static ScaleformData1.CInlinedArray* ScaleformData1Array { get; private set; }
        public static ScaleformData2.CArray* ScaleformData2Array { get; private set; }
        public static CTimeCycleModifiersManager* TimeCycleModifiersManager { get; private set; }
        public static sysMemAllocator* Allocator { get; private set; }

        internal static bool Init()
        {
            IntPtr address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B 45 EC 4C 8D 45 F0 48 8D 55 EC 48 8D 0D");
            if (AssertAddress(address, nameof(fwTxdStore)))
            {
                address = address + *(int*)(address + 3) + 7;
                TxdStore = (fwTxdStore*)address;
            }

            address = Game.FindPattern("48 8B 0D ?? ?? ?? ?? C7 44 24 ?? ?? ?? ?? ?? 89 44 24 68 33 C0 89 44 24 30");
            if (AssertAddress(address, nameof(CAnimPostFXManager)))
            {
                address = address + *(int*)(address + 3) + 7;
                AnimPostFXManager = *(CAnimPostFXManager**)address;
            }

            address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 8B D3 E8 ?? ?? ?? ?? 84 C0 74 18");
            if (AssertAddress(address, nameof(CScaleformStore)))
            {
                address = address + *(int*)(address + 3) + 7;
                ScaleformStore = (CScaleformStore*)address;
            }

            address = Game.FindPattern("48 8D 35 ?? ?? ?? ?? 48 8D 3C C0 8B 4C FE B8 E8 ?? ?? ?? ?? 84 C0 74 15");
            if (AssertAddress(address, $"{nameof(ScaleformData1)}.{nameof(ScaleformData1.CInlinedArray)}"))
            {
                address = address + *(int*)(address + 3) + 7;
                ScaleformData1Array = (ScaleformData1.CInlinedArray*)address;
            }

            address = Game.FindPattern("48 8B 0D ?? ?? ?? ?? 48 69 D2 ?? ?? ?? ?? 83 BC 0A ?? ?? ?? ?? ?? 0F 94 C0 C3");
            if (AssertAddress(address, $"{nameof(ScaleformData2)}.{nameof(ScaleformData2.CArray)}"))
            {
                address = address + *(int*)(address + 3) + 7;
                ScaleformData2Array = (ScaleformData2.CArray*)address;
            }

            address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? 45 33 C9 89 44 24 38 E8 ?? ?? ?? ?? 83 0D");
            if (AssertAddress(address, nameof(CTimeCycleModifiersManager)))
            {
                address = address + *(int*)(address + 3) + 7;
                TimeCycleModifiersManager = (CTimeCycleModifiersManager*)address;
            }

            address = Game.FindPattern("48 8D 1D ?? ?? ?? ?? A8 08 75 1D 83 C8 08 48 8B CB 89 05 ?? ?? ?? ??");
            if (AssertAddress(address, nameof(sysMemAllocator)))
            {
                address = address + *(int*)(address + 3) + 7;
                Allocator = (sysMemAllocator*)address;
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

