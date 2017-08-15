namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    // defined in animpostfx.ymt
    public unsafe sealed class PostFxAnimation : IAddressable
    {
        private uint hash;
        private CAnimPostFX* native;
        private int index = -1;

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
                    return IsValid() && (GameFunctions.IsAnimPostFXActive(GameMemory.AnimPostFXManager, hashPtr) & 1) != 0;
                }
            }
        }

        public IntPtr MemoryAddress { get { return (IntPtr)native; } }
        public int Index { get { return index; } }
        public LayerBlend Blend { get; }
        public LayersWrapper Layers { get; }

        private PostFxAnimation(CAnimPostFX* native)
        {
            hash = native->Name;
            this.native = native;
            if(IsValid())
            {
                index = unchecked((int)((long)native - (long)GameMemory.AnimPostFXManager->Effects.Offset) / sizeof(CAnimPostFX));
            }
            Blend = new LayerBlend(this);
            Layers = new LayersWrapper(this);
        }

        public bool IsValid()
        {
            return MemoryAddress != IntPtr.Zero;
        }

        public void Start(int duration, bool looped)
        {
            if (!IsValid())
                return;

            fixed (uint* hashPtr = &hash)
            {
                GameFunctions.StartAnimPostFX(GameMemory.AnimPostFXManager, hashPtr, duration, looped, 0, 0, 0);
            }
        }

        public void Stop()
        {
            if (!IsValid())
                return;

            fixed (uint* hashPtr = &hash)
            {
                GameFunctions.StopAnimPostFX(GameMemory.AnimPostFXManager, hashPtr);
            }
        }


        public static void StopAll()
        {
            NativeFunction.Natives.xB4EDDC19532BFB85(); // _STOP_ALL_SCREEN_EFFECTS
        }

        public static PostFxAnimation GetByName(string name)
        {
            uint hash = Game.GetHashKey(name);
            knownNames[hash] = name;
            return GetByHash(hash);
        }

        public static PostFxAnimation GetByHash(uint hash)
        {
            if (cache.TryGetValue(hash, out PostFxAnimation p))
            {
                return p;
            }
            else
            {
                CAnimPostFX* native = GameFunctions.GetAnimPostFXByHash(GameMemory.AnimPostFXManager, &hash);

                if (native != null)
                {
                    PostFxAnimation e = new PostFxAnimation(native);
                    cache[hash] = e;
                    return e;
                }
            }

            return null;
        }

        public static PostFxAnimation GetByIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            short i = (short)index;
            CAnimPostFX* native = GameMemory.AnimPostFXManager->Effects.Get(i);

            if (native != null)
            {
                if (cache.TryGetValue(native->Name, out PostFxAnimation p))
                {
                    return p;
                }
                else
                {
                    PostFxAnimation e = new PostFxAnimation(native);
                    cache[native->Name] = e;
                    return e;
                }
            }

            return null;
        }

        public static PostFxAnimation[] GetAll()
        {
            PostFxAnimation[] effects = new PostFxAnimation[GameMemory.AnimPostFXManager->Effects.Count];
            for (short i = 0; i < GameMemory.AnimPostFXManager->Effects.Count; i++)
            {
                CAnimPostFX* e = GameMemory.AnimPostFXManager->Effects.Get(i);
                effects[i] = GetByHash(e->Name);
            }

            return effects;
        }
        
        public static PostFxAnimation LastAnimation
        {
            get
            {
                CAnimPostFX* e = GameMemory.AnimPostFXManager->GetLastActiveEffect();
                if (e != null)
                {
                    return GetByHash(e->Name);
                }

                return null;
            }
        }

        public static PostFxAnimation CurrentAnimation
        {
            get
            {
                CAnimPostFX* e = GameMemory.AnimPostFXManager->GetCurrentActiveEffect();
                if (e != null)
                {
                    return GetByHash(e->Name);
                }

                return null;
            }
        }

        public static int Count
        {
            get
            {
                return GameMemory.AnimPostFXManager->Effects.Count;
            }
        }

        private static Dictionary<uint, PostFxAnimation> cache = new Dictionary<uint, PostFxAnimation>();
        
        public sealed class LayerBlend : IAddressable
        {
            private PostFxAnimation animation;
            private CAnimPostFX.LayerBlend* native;

            public IntPtr MemoryAddress { get { return (IntPtr)native; } }

            private Layer layerA;
            public Layer LayerA
            {
                get
                {
                    if(native->LayerA == null)
                    {
                        return null;
                    }
                    else
                    {
                        if(layerA != null)
                        {
                            return layerA;
                        }
                        else
                        {
                            for (int i = 0; i < animation.Layers.Count; i++)
                            {
                                if(animation.Layers[i].MemoryAddress == (IntPtr)native->LayerA)
                                {
                                    layerA = animation.Layers[i];
                                    break;
                                }
                            }

                            return layerA;
                        }
                    }
                }
            }

            private Layer layerB;
            public Layer LayerB
            {
                get
                {
                    if (native->LayerB == null)
                    {
                        return null;
                    }
                    else
                    {
                        if (layerB != null)
                        {
                            return layerB;
                        }
                        else
                        {
                            for (int i = 0; i < animation.Layers.Count; i++)
                            {
                                if (animation.Layers[i].MemoryAddress == (IntPtr)native->LayerB)
                                {
                                    layerB = animation.Layers[i];
                                    break;
                                }
                            }

                            return layerB;
                        }
                    }
                }
            }

            public float FrequencyNoise { get { return native->FrequencyNoise; } }
            public float AmplitudeNoise { get { return native->AmplitudeNoise; } }
            public float Frequency { get { return native->Frequency; } }
            public float Bias { get { return native->Bias; } }

            public bool Disabled { get { return native->Disabled == 1; } }
            
            internal LayerBlend(PostFxAnimation anim)
            {
                animation = anim;
                native = &animation.native->FXStack.LayerBlend;
            }

            public bool IsValid()
            {
                return MemoryAddress != IntPtr.Zero;
            }
        }

        public sealed class LayersWrapper
        {
            private PostFxAnimation animation;
            private Layer[] layers;

            public int Count { get { return animation.native->FXStack.LayersCount; } }
            public Layer this[int index]
            {
                get
                {
                    if (index < 0 || index >= Count)
                        throw new IndexOutOfRangeException();

                    if(layers[index] == null)
                    {
                        Layer l = new Layer(animation.native->FXStack.GetLayer(index));
                        layers[index] = l;
                        return l;
                    }
                    else
                    {
                        return layers[index];
                    }
                }
            }

            internal LayersWrapper(PostFxAnimation anim)
            {
                animation = anim;
                layers = new Layer[Count];
            }
        }

        public sealed class Layer : IAddressable
        {
            private CAnimPostFX.Layer* native;

            public IntPtr MemoryAddress { get { return (IntPtr)native; } }

            public TimeCycleModifier Modifier
            {
                get
                {
                    return native->ModifierName == 0 ? null : TimeCycleModifier.GetByHash(native->ModifierName);
                }
            }

            public uint StartDelayDuration { get { return native->StartDelayDuration; } }
            public uint InDuration { get { return native->InDuration; } }
            public uint HoldDuration { get { return native->HoldDuration; } }
            public uint OutDuration { get { return native->OutDuration; } }
            public AnimationMode AnimationMode { get { return (AnimationMode)native->AnimMode; } }
            public LoopMode LoopMode { get { return (LoopMode)native->LoopMode; } }

            internal Layer(CAnimPostFX.Layer* native)
            {
                this.native = native;
            }

            public bool IsValid()
            {
                return MemoryAddress != IntPtr.Zero;
            }
        }

        public enum AnimationMode : uint
        {
            InHoldOut = 0, // POSTFX_IN_HOLD_OUT
            EaseInHoldEaseOut = 1, // POSTFX_EASE_IN_HOLD_EASE_OUT
            EaseIn = 2, // POSTFX_EASE_IN
        }

        public enum LoopMode : uint
        { 
            HoldOnly = 1, // POSTFX_LOOP_HOLD_ONLY
            None = 2, // POSTFX_LOOP_NONE
        }

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
            { 0x10493196, "SwitchHUDMichaelIn" },
            { 0x7E17B1A, "SwitchHUDFranklinIn" },
            { 0xEE33C206, "SwitchHUDTrevorIn" },
            { 0x19035B64, "SwitchShortNeutralMid"},
            { 0xE5CAFC5B, "SwitchOpenNeutralIn" },
            { 0xF047EDA2, "DrugsFalling01"},
            { 0xDDECD135, "DefaultFlash"},
            { 0xB93C1D74, "DefaultLensDistortion"},
            { 0x53F1A741, "DefaultColorGrade"},
            { 0x5E05AE17, "DefaultMenuFadeIn"},
            { 0x6014108B, "DefaultMenuFadeOut"},
            { 0x1074AD81, "DeathFailFranklinIn"},
            { 0x19509151, "DeathFailMichaelIn"},
            { 0x2FB801ED, "DeathFailTrevorIn"},
            { 0x44DABE22, "PauseMenuIn"},
            { 0x725941F7, "PauseMenuOut"},
            { 0x80802E11, "PauseMenuFranklinIn"},
            { 0x388A0486, "PauseMenuFranklinOut"},
            { 0xA0544755, "PauseMenuMichaelIn"},
            { 0x9CBEEBEE, "PauseMenuMichaelOut"},
            { 0xB3ED728, "PauseMenuTrevorIn"},
            { 0x24239EF8, "PauseMenuTrevorOut"},
            { 0x4FB4576D, "MenuMGIn"},
            { 0xFFEC2719, "MenuMGOut"},
            { 0xE84C01AF, "MenuGMFranklinIn"},
            { 0xBB72A08A, "MenuMGFranklinOut"},
            { 0x615EAA30, "MenuMGMichaelIn"},
            { 0x63F04889, "MenuMGMichaelOut"},
            { 0xF3EA9B0, "MenuMGTrevorIn"},
            { 0xE627C940, "MenuMGTrevorOut"},
            { 0xC36D571F, "SwitchOpenNeutral"},
            { 0x22D04949, "SwitchOpenFranklinMid"},
            { 0x621BE8F8, "SwitchOpenMichaelMid"},
            { 0xFB80DB62, "SwitchOpenTrevorMid"},
            { 0x6CBDD92D, "SwitchOpenNeutralMid"},
            { 0x1BAE2D29, "SwitchOpenFranklinOut"},
            { 0x15DCA17B, "SwitchOpenMichaelOut"},
            { 0x87F585EB, "SwitchOpenTrevorOut"},
            { 0x2977920C, "SwitchOpenNeutralOut"},
            { 0x6F193857, "RespawnFranklin"},
            { 0x1DF47FE2, "RespawnMichael"},
            { 0x7E421BFE, "RespawnTrevor"},
            { 0x2A135C43, "DrivingFocus"},
            { 0x6E5C08AD, "DrivingFocusOut"},
            { 0x446DD766, "BulletTime"},
            { 0xD2427E1, "BulletTimeOut"},
            { 0xE4357941, "RedMist"},
            { 0x8889C8B6, "RedMistOut"},
            { 0x4F8EDF17, "TestBlend"},
            { 0x2AC16965, "DefaultMPScreenFade"},
            { 0x9CFB4C6E, "PedGunKill"},
            { 0xA99718C2, "PhoneCameraIn"},
            { 0xA51FD197, "MP_Bull_Tost"},
            { 0xE0940793, "MP_Killstreak"},
            { 0xAB722F6A, "MP_Powerplay"},
            { 0xE7590770, "DefaultBlinkIntro"},
            { 0x22D8DCE3, "DefaultBlinkOutro"},
            { 0xD7B0BF33, "InchPurple"},
            { 0xF2045A0E, "InchOrange"},
            { 0x166FF30B, "BeastLaunch"},
            { 0x6B66A555, "CrossLine"},
            { 0x5AAE758F, "PennedIn"},
            { 0xE26BE615, "InchPickup"},
        };
    }
}

