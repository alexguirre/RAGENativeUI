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
                if (KnownNames.PostFxAnimations.Dictionary.TryGetValue(hash, out string n))
                {
                    return n;
                }

                return $"0x{hash.ToString("X8")}";
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
            KnownNames.PostFxAnimations.Dictionary[hash] = name;
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
    }
}

