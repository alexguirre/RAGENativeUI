namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    internal static unsafe class GameFunctions
    {
        public delegate bool StartPostFXDelegate(CPostFXManager* effectMgr, uint* effectNameHashPtr, int duration, bool looped, byte a5, int a6, int a7);
        public delegate void StopPostFXDelegate(CPostFXManager* effectMgr, uint* effectNameHashPtr);
        public delegate long IsPostFXActiveDelegate(CPostFXManager* effectMgr, uint* effectNameHashPtr);
        public delegate CPostFX* GetPostFXByHashDelegate(CPostFXManager* effectMgr, uint* effectNameHashPtr);
        
        public static StartPostFXDelegate StartPostFX { get; private set; }
        public static StopPostFXDelegate StopPostFX { get; private set; }
        public static IsPostFXActiveDelegate IsPostFXActive { get; private set; }
        public static GetPostFXByHashDelegate GetPostFXByHash { get; private set; }

        internal static bool Init()
        {
            IntPtr address = Game.FindPattern("48 89 5C 24 ?? 48 89 6C 24 ?? 56 57 41 56 48 83 EC 20 8B 02 48 8B DA 48 8D 54 24 ??");
            if (AssertAddress(address, nameof(StartPostFX)))
            {
                StartPostFX = Marshal.GetDelegateForFunctionPointer<StartPostFXDelegate>(address);
            }
            
            address = Game.FindPattern("40 53 48 83 EC 20 8B 02 48 8D 54 24 ?? 48 8B D9 89 44 24 38 E8 ?? ?? ?? ?? 48 8B D0 48 85 C0");
            if (AssertAddress(address, nameof(StopPostFX)))
            {
                StopPostFX = Marshal.GetDelegateForFunctionPointer<StopPostFXDelegate>(address);
            }

            address = Game.FindPattern("40 53 48 83 EC 20 8B 02 48 8D 54 24 ?? 89 44 24 38");
            if (AssertAddress(address, nameof(IsPostFXActive)))
            {
                IsPostFXActive = Marshal.GetDelegateForFunctionPointer<IsPostFXActiveDelegate>(address);
            }

            address = Game.FindPattern("0F B7 41 08 45 33 C0 45 8B C8 44 8B D0 85 C0 7E 1A 8B 12");
            if (AssertAddress(address, nameof(GetPostFXByHash)))
            {
                GetPostFXByHash = Marshal.GetDelegateForFunctionPointer<GetPostFXByHashDelegate>(address);
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

