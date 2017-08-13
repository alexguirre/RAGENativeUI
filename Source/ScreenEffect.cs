namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    public unsafe sealed class ScreenEffect : IAddressable
    {
        private uint hash;
        private IntPtr memAddress;

        public uint Hash { get { return hash; } }
        public string Name
        {
            get
            {
                if (knownNames.TryGetValue(hash, out string n))
                {
                    return n;
                }

                return $"0x{hash.ToString("X")}";
            }
        }

        public bool IsActive
        {
            get
            {
                fixed (uint* hashPtr = &hash)
                {
                    return IsValid() && (GameFunctions.IsScreenEffectActive(GameMemory.ScreenEffectsManager, hashPtr) & 1) != 0;
                }
            }
        }

        public IntPtr MemoryAddress { get { return memAddress; } }

        private ScreenEffect(uint hash)
        {
            this.hash = hash;
            memAddress = (IntPtr)GameFunctions.GetScreenEffectByHash(GameMemory.ScreenEffectsManager, &hash);
        }

        public bool IsValid()
        {
            return memAddress != IntPtr.Zero;
        }

        public void Start(int duration, bool looped)
        {
            if (!IsValid())
                return;

            fixed (uint* hashPtr = &hash)
            {
                GameFunctions.StartScreenEffect(GameMemory.ScreenEffectsManager, hashPtr, duration, looped, 0, 0, 0);
            }
        }

        public void Stop()
        {
            if (!IsValid())
                return;

            fixed (uint* hashPtr = &hash)
            {
                GameFunctions.StopScreenEffect(GameMemory.ScreenEffectsManager, hashPtr);
            }
        }


        public static void StopAll()
        {
            NativeFunction.Natives.xB4EDDC19532BFB85(); // _STOP_ALL_SCREEN_EFFECTS
        }

        public static ScreenEffect GetByName(string name)
        {
            uint hash = Game.GetHashKey(name);
            knownNames[hash] = name;
            return GetByHash(hash);
        }

        public static ScreenEffect GetByHash(uint hash)
        {
            if (cache.TryGetValue(hash, out ScreenEffect se))
            {
                return se;
            }
            else
            {
                ScreenEffect e = new ScreenEffect(hash);
                if (e.IsValid())
                {
                    cache[hash] = e;
                    return e;
                }
            }

            return null;
        }

        public static ScreenEffect[] GetAll()
        {
            ScreenEffect[] effects = new ScreenEffect[GameMemory.ScreenEffectsManager->Effects.Count];
            for (short i = 0; i < GameMemory.ScreenEffectsManager->Effects.Count; i++)
            {
                CScreenEffect* e = GameMemory.ScreenEffectsManager->Effects.Get(i);
                effects[i] = GetByHash(e->NameHash);
            }

            return effects;
        }
        
        public static ScreenEffect LastActive
        {
            get
            {
                CScreenEffect* e = GameMemory.ScreenEffectsManager->GetLastActiveEffect();
                if(e != null)
                {
                    return GetByHash(e->NameHash);
                }

                return null;
            }
        }

        public static ScreenEffect CurrentActive
        {
            get
            {
                ScreenEffect e = LastActive;

                return (e == null || !e.IsActive) ? null : e;
            }
        }

        private static Dictionary<uint, ScreenEffect> cache = new Dictionary<uint, ScreenEffect>();

        // TODO: find if ScreenEffects names can be retrieved from memory
        private static Dictionary<uint, string> knownNames = new Dictionary<uint, string>()
        {
            { 0xB2895E1B, "SwitchHUDIn" },
            { 0x9AC7662A, "SwitchHUDOut" },
            { 0x99D8EF9, "FocusIn" },
            { 0xD8664E89, "FocusOut" },
            { 0x507BF7B6, "MinigameEndNeutral" },
            { 0x4C46C8B1, "MinigameEndTrevor" },
            { 0xA384D25B, "MinigameEndFranklin" },
            { 0xFD306DF8, "MinigameEndMichael" },
            { 0x2A7B10CA, "MinigameTransitionOut" },
            { 0xBB300FB8, "MinigameTransitionIn" },
            { 0x66FCFD3E, "SwitchShortNeutralIn" },
            { 0xC2225603, "SwitchShortFranklinIn" },
            { 0xD91BDD3, "SwitchShortTrevorIn" },
            { 0x10FB8EB6, "SwitchShortMichaelIn" },
            { 0x848674FD, "SwitchOpenMichaelIn" },
            { 0x776FE6F3, "SwitchOpenFranklinIn" },
            { 0x1A4E629D, "SwitchOpenTrevorIn" },
            { 0x2FE80A59, "SwitchHUDMichaelOut" },
            { 0x106D489B, "SwitchHUDFranklinOut" },
            { 0x8C59987, "SwitchHUDTrevorOut" },
            { 0x6BA28347, "SwitchShortFranklinMid" },
            { 0x8A7D2D1B, "SwitchShortMichaelMid" },
            { 0x67AE7F68, "SwitchShortTrevorMid" },
            { 0x4AC9635A, "DeathFailOut" },
            { 0x15B714F1, "CamPushInNeutral" },
            { 0xEE41DACB, "CamPushInFranklin" },
            { 0xB040F488, "CamPushInMichael" },
            { 0xE3BB0589, "CamPushInTrevor" },
            { 0x66FB30FE, "SwitchSceneFranklin" },
            { 0xEE02C950, "SwitchSceneTrevor" },
            { 0x2F8758E3, "SwitchSceneMichael" },
            { 0x109E3F90, "SwitchSceneNeutral" },
            { 0xD4851430, "MP_Celeb_Win" },
            { 0xFDE9AC37, "MP_Celeb_Win_Out" },
            { 0x1E30FAFC, "MP_Celeb_Lose" },
            { 0xFE208E24, "MP_Celeb_Lose_Out" },
            { 0x7842C770, "DeathFailNeutralIn" },
            { 0x1D904505, "DeathFailMPDark" },
            { 0x4FA8A25F, "DeathFailMPIn" },
            { 0xEE6B9EFE, "MP_Celeb_Preload_Fade" },
            { 0xF878A71B, "PeyoteEndOut" },
            { 0x91304B90, "PeyoteEndIn" },
            { 0x6A7B1294, "PeyoteIn" },
            { 0xE7E4F08C, "PeyoteOut" },
            { 0xD78F4146, "MP_race_crash" },
            { 0x33460F26, "SuccessFranklin" },
            { 0x96631BD8, "SuccessTrevor" },
            { 0x4C589541, "SuccessMichael" },
            { 0x9A2D54ED, "DrugsMichaelAliensFightIn" },
            { 0x94BC475, "DrugsMichaelAliensFight" },
            { 0xDA34F570, "DrugsMichaelAliensFightOut" },
            { 0x25CF4D3C, "DrugsTrevorClownsFightIn" },
            { 0x501D990E, "DrugsTrevorClownsFight" },
            { 0xA7E51270, "DrugsTrevorClownsFightOut" },
            { 0x157CD80E, "HeistCelebPass" },
            { 0x57FB1394, "HeistCelebPassBW" },
            { 0x8B2E6021, "HeistCelebEnd" },
            { 0x48133C7A, "HeistCelebToast" },
            { 0xF21B4A96, "MenuMGHeistIn" },
            { 0x3922C96, "MenuMGTournamentIn" },
            { 0x484732C5, "MenuMGSelectionIn" },
            { 0x29A04AB8, "ChopVision" },
            { 0x3C50ABD4, "DMT_flight_intro" },
            { 0x45200EDC, "DMT_flight" },
            { 0x8B05DF27, "DrugsDrivingIn" },
            { 0xD8D246A4, "DrugsDrivingOut" },
            { 0x4A4777E7, "SwitchOpenNeutralFIB5" },
            { 0x9ECCB53F, "HeistLocate" },
            { 0x9754FCB6, "MP_job_load" },
            { 0x600D9D85, "RaceTurbo" },
            { 0x9C649D97, "MP_intro_logo" },
            { 0xE9D35B50, "HeistTripSkipFade" },
            { 0x7765CDB8, "MenuMGHeistOut" },
            { 0x19A820C2, "MP_corona_switch" },
            { 0xBF31D7C1, "MenuMGSelectionTint" },
            { 0xE59492AF, "SuccessNeutral" },
            { 0xB764F089, "ExplosionJosh3" },
            { 0x142AF978, "SniperOverlay" },
            { 0xA38FD6BA, "RampageOut" },
            { 0x99BE14C5, "Rampage" },
            { 0xB766CC15, "Dont_tazeme_bro" },
        };
    }
}

