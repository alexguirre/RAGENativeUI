namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;
    
    internal static unsafe class GameFunctions
    {
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate bool StartAnimPostFXDelegate(ref CAnimPostFXManager animPostFXMgr, in uint effectNameHashPtr, int duration, [MarshalAs(UnmanagedType.I1)] bool looped, byte a5, int a6, int a7);
        public delegate void StopAnimPostFXDelegate(ref CAnimPostFXManager animPostFXMgr, in uint effectNameHashPtr);
        [return: MarshalAs(UnmanagedType.I1)]
        public delegate bool IsAnimPostFXActiveDelegate(ref CAnimPostFXManager animPostFXMgr, in uint effectNameHashPtr);
        public delegate Ptr GetAnimPostFXByHashDelegate(ref CAnimPostFXManager animPostFXMgr, in uint effectNameHashPtr);
        public delegate int GetTimeCycleModifierIndexDelegate(ref CTimeCycle timeCycle, in uint nameHash);


        public static StartAnimPostFXDelegate StartAnimPostFX { get; private set; }
        public static StopAnimPostFXDelegate StopAnimPostFX { get; private set; }
        public static IsAnimPostFXActiveDelegate IsAnimPostFXActive { get; private set; }
        public static GetAnimPostFXByHashDelegate GetAnimPostFXByHash { get; private set; }
        public static GetTimeCycleModifierIndexDelegate GetTimeCycleModifierIndex { get; private set; }

        internal static bool Init()
        {
            IntPtr address = RPH.Game.FindPattern("48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC 20 8B 02 48 8B DA 48 8D 54 24 ??");
            if (AssertAddress(address, nameof(StartAnimPostFX)))
            {
                StartAnimPostFX = Marshal.GetDelegateForFunctionPointer<StartAnimPostFXDelegate>(address);
            }
            
            address = RPH.Game.FindPattern("40 53 48 83 EC 20 8B 02 48 8D 54 24 ?? 48 8B D9 89 44 24 38 E8 ?? ?? ?? ?? 48 8B D0 48 85 C0");
            if (AssertAddress(address, nameof(StopAnimPostFX)))
            {
                StopAnimPostFX = Marshal.GetDelegateForFunctionPointer<StopAnimPostFXDelegate>(address);
            }

            address = RPH.Game.FindPattern("40 53 48 83 EC 20 8B 02 48 8D 54 24 ?? 89 44 24 38");
            if (AssertAddress(address, nameof(IsAnimPostFXActive)))
            {
                IsAnimPostFXActive = Marshal.GetDelegateForFunctionPointer<IsAnimPostFXActiveDelegate>(address);
            }

            address = RPH.Game.FindPattern("0F B7 41 08 45 33 C0 45 8B C8 44 8B D0 85 C0 7E 1A 8B 12");
            if (AssertAddress(address, nameof(GetAnimPostFXByHash)))
            {
                GetAnimPostFXByHash = Marshal.GetDelegateForFunctionPointer<GetAnimPostFXByHashDelegate>(address);
            }

            address = RPH.Game.FindPattern("40 53 48 83 EC 20 8B 02 48 8D 54 24 ?? 48 8B D9 89 44 24 30 E8 ?? ?? ?? ?? 0F B7 53 48");
            if (AssertAddress(address, nameof(GetTimeCycleModifierIndex)))
            {
                GetTimeCycleModifierIndex = Marshal.GetDelegateForFunctionPointer<GetTimeCycleModifierIndexDelegate>(address);
            }

            return !anyAssertFailed;
        }

        private static bool anyAssertFailed = false;
        private static bool AssertAddress(IntPtr address, string name)
        {
            if (address == IntPtr.Zero)
            {
                Common.Log($"Incompatible game version, couldn't find {name} function address.");
                anyAssertFailed = true;
                return false;
            }

            return true;
        }
    }
}

