namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    // defined in animpostfx.ymt
    public unsafe sealed class PostFxAnimation : IAddressable
    {
        private uint hash; // not readonly to be able to take its address in other methods
        private readonly CAnimPostFX* native;
        private readonly int index = -1;

        public uint Hash { get { return hash; } }
        public string Name
        {
            get
            {
                if (KnownNames.PostFxAnimations.Dictionary.TryGetValue(hash, out string n))
                {
                    return n;
                }

                return $"0x{hash:X8}";
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
        public PostFxAnimationLayerBlend LayerBlend { get; }
        public PostFxAnimationLayersCollection Layers { get; }

        private PostFxAnimation(CAnimPostFX* native)
        {
            hash = native->Name;
            this.native = native;
            if(IsValid())
            {
                index = unchecked((int)((long)native - (long)GameMemory.AnimPostFXManager->Effects.Offset) / sizeof(CAnimPostFX));
            }
            LayerBlend = new PostFxAnimationLayerBlend(this);
            Layers = new PostFxAnimationLayersCollection(this);
        }

        internal CAnimPostFX* GetNative() => native;

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
            Throw.IfNull(name, nameof(name));

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
            Throw.IfOutOfRange(index, 0, Count - 1, nameof(index));

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
    }

    public enum PostFxAnimationLayerAnimationMode : uint
    {
        InHoldOut = 0, // POSTFX_IN_HOLD_OUT
        EaseInHoldEaseOut = 1, // POSTFX_EASE_IN_HOLD_EASE_OUT
        EaseIn = 2, // POSTFX_EASE_IN
    }

    public enum PostFxAnimationLayerLoopMode : uint
    {
        HoldOnly = 1, // POSTFX_LOOP_HOLD_ONLY
        None = 2, // POSTFX_LOOP_NONE
    }

    public unsafe sealed class PostFxAnimationLayer : IAddressable
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
        public PostFxAnimationLayerAnimationMode AnimationMode { get { return (PostFxAnimationLayerAnimationMode)native->AnimMode; } }
        public PostFxAnimationLayerLoopMode LoopMode { get { return (PostFxAnimationLayerLoopMode)native->LoopMode; } }

        internal PostFxAnimationLayer(CAnimPostFX.Layer* native)
        {
            this.native = native;
        }

        public bool IsValid()
        {
            return MemoryAddress != IntPtr.Zero;
        }
    }

    public unsafe sealed class PostFxAnimationLayersCollection : IReadOnlyCollection<PostFxAnimationLayer>, IEnumerable<PostFxAnimationLayer>
    {
        private PostFxAnimation animation;
        private PostFxAnimationLayer[] layers;

        public int Count { get { return animation.GetNative()->FXStack.LayersCount; } }
        public PostFxAnimationLayer this[int index]
        {
            get
            {
                Throw.IfOutOfRange(index, 0, Count - 1, nameof(index));

                if (layers[index] == null)
                {
                    PostFxAnimationLayer l = new PostFxAnimationLayer(animation.GetNative()->FXStack.GetLayer(index));
                    layers[index] = l;
                    return l;
                }
                else
                {
                    return layers[index];
                }
            }
        }

        internal PostFxAnimationLayersCollection(PostFxAnimation anim)
        {
            animation = anim;
            layers = new PostFxAnimationLayer[Count];
        }

        public IEnumerator<PostFxAnimationLayer> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public unsafe sealed class PostFxAnimationLayerBlend : IAddressable
    {
        private PostFxAnimation animation;
        private CAnimPostFX.LayerBlend* native;

        public IntPtr MemoryAddress { get { return (IntPtr)native; } }

        private PostFxAnimationLayer layerA;
        public PostFxAnimationLayer LayerA
        {
            get
            {
                if (native->LayerA == null)
                {
                    return null;
                }
                else
                {
                    if (layerA != null)
                    {
                        return layerA;
                    }
                    else
                    {
                        for (int i = 0; i < animation.Layers.Count; i++)
                        {
                            if (animation.Layers[i].MemoryAddress == (IntPtr)native->LayerA)
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

        private PostFxAnimationLayer layerB;
        public PostFxAnimationLayer LayerB
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

        internal PostFxAnimationLayerBlend(PostFxAnimation anim)
        {
            animation = anim;
            native = &animation.GetNative()->FXStack.LayerBlend;
        }

        public bool IsValid()
        {
            return MemoryAddress != IntPtr.Zero;
        }
    }
}

