namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    // defined in animpostfx.ymt
    public unsafe sealed class PostFxAnimation : IAddressable
    {
        private uint hash; // not readonly to be able to take its address in other methods
        private readonly IntPtr memAddress;
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
                return IsValid() && (GameFunctions.IsAnimPostFXActive(ref GameMemory.AnimPostFXManager, ref hash) & 1) != 0;
            }
        }

        public IntPtr MemoryAddress { get { return memAddress; } }
        public int Index { get { return index; } }
        public PostFxAnimationLayerBlend LayerBlend { get; }
        public PostFxAnimationLayersCollection Layers { get; }

        private PostFxAnimation(ref CAnimPostFX native)
        {
            hash = native.Name;
            memAddress = (IntPtr)Unsafe.AsPointer(ref native);
            if(IsValid())
            {
                index = unchecked((int)((long)memAddress - (long)GameMemory.AnimPostFXManager.Effects.Offset) / Unsafe.SizeOf<CAnimPostFX>());
            }
            LayerBlend = new PostFxAnimationLayerBlend(this);
            Layers = new PostFxAnimationLayersCollection(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref CAnimPostFX GetNative() => ref Unsafe.AsRef<CAnimPostFX>(memAddress.ToPointer());
        
        public bool IsValid()
        {
            return MemoryAddress != IntPtr.Zero;
        }

        public void Start(int duration, bool looped)
        {
            if (!IsValid())
                return;

            GameFunctions.StartAnimPostFX(ref GameMemory.AnimPostFXManager, ref hash, duration, looped, 0, 0, 0);
        }

        public void Stop()
        {
            if (!IsValid())
                return;

            GameFunctions.StopAnimPostFX(ref GameMemory.AnimPostFXManager, ref hash);
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
                IntPtr native = GameFunctions.GetAnimPostFXByHash(ref GameMemory.AnimPostFXManager, ref hash);

                if (native != IntPtr.Zero)
                {
                    PostFxAnimation e = new PostFxAnimation(ref Unsafe.AsRef<CAnimPostFX>(native.ToPointer()));
                    cache[hash] = e;
                    return e;
                }
            }

            return null;
        }

        public static PostFxAnimation GetByIndex(int index)
        {
            Throw.IfOutOfRange(index, 0, NumberOfPostFxAnimations - 1, nameof(index));

            short i = (short)index;
            ref CAnimPostFX native = ref GameMemory.AnimPostFXManager.Effects[i];

            if (cache.TryGetValue(native.Name, out PostFxAnimation p))
            {
                return p;
            }
            else
            {
                PostFxAnimation e = new PostFxAnimation(ref native);
                cache[native.Name] = e;
                return e;
            }
        }

        public static PostFxAnimation[] GetAll()
        {
            ref CAnimPostFXManager mgr = ref GameMemory.AnimPostFXManager;
            PostFxAnimation[] effects = new PostFxAnimation[mgr.Effects.Count];
            for (short i = 0; i < mgr.Effects.Count; i++)
            {
                ref CAnimPostFX e = ref mgr.Effects[i];
                effects[i] = GetByHash(e.Name);
            }

            return effects;
        }
        
        public static PostFxAnimation LastAnimation
        {
            get
            {
                void* e = GameMemory.AnimPostFXManager.GetLastActiveEffect();
                if (e != null)
                {
                    return GetByHash(Unsafe.AsRef<CAnimPostFX>(e).Name);
                }

                return null;
            }
        }

        public static PostFxAnimation CurrentAnimation
        {
            get
            {
                void* e = GameMemory.AnimPostFXManager.GetCurrentActiveEffect();
                if (e != null)
                {
                    return GetByHash(Unsafe.AsRef<CAnimPostFX>(e).Name);
                }

                return null;
            }
        }

        public static int NumberOfPostFxAnimations
        {
            get
            {
                return GameMemory.AnimPostFXManager.Effects.Count;
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
        private readonly IntPtr memAddress;

        public IntPtr MemoryAddress { get { return memAddress; } }

        public TimeCycleModifier Modifier
        {
            get
            {
                ref CAnimPostFX.Layer l = ref GetNative();
                return l.ModifierName == 0 ? null : TimeCycleModifier.GetByHash(l.ModifierName);
            }
        }

        public uint StartDelayDuration { get { return GetNative().StartDelayDuration; } }
        public uint InDuration { get { return GetNative().InDuration; } }
        public uint HoldDuration { get { return GetNative().HoldDuration; } }
        public uint OutDuration { get { return GetNative().OutDuration; } }
        public PostFxAnimationLayerAnimationMode AnimationMode { get { return (PostFxAnimationLayerAnimationMode)GetNative().AnimMode; } }
        public PostFxAnimationLayerLoopMode LoopMode { get { return (PostFxAnimationLayerLoopMode)GetNative().LoopMode; } }

        internal PostFxAnimationLayer(ref CAnimPostFX.Layer native)
        {
            memAddress = (IntPtr)Unsafe.AsPointer(ref native);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref CAnimPostFX.Layer GetNative() => ref Unsafe.AsRef<CAnimPostFX.Layer>(memAddress.ToPointer());

        public bool IsValid()
        {
            return MemoryAddress != IntPtr.Zero;
        }
    }

    public unsafe sealed class PostFxAnimationLayersCollection : IReadOnlyCollection<PostFxAnimationLayer>, IEnumerable<PostFxAnimationLayer>
    {
        private PostFxAnimation animation;
        private PostFxAnimationLayer[] layers;

        public int Count { get { return animation.GetNative().FXStack.LayersCount; } }
        public PostFxAnimationLayer this[int index]
        {
            get
            {
                Throw.IfOutOfRange(index, 0, Count - 1, nameof(index));

                if (layers[index] == null)
                {
                    PostFxAnimationLayer l = new PostFxAnimationLayer(ref animation.GetNative().FXStack.Layers[index]);
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
        private readonly PostFxAnimation animation;
        private readonly IntPtr memAddress;

        public IntPtr MemoryAddress { get { return memAddress; } }

        private PostFxAnimationLayer layerA;
        public PostFxAnimationLayer LayerA
        {
            get
            {
                if (GetNative().LayerA == null)
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
                            if (animation.Layers[i].MemoryAddress == (IntPtr)GetNative().LayerA)
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
                if (GetNative().LayerB == null)
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
                            if (animation.Layers[i].MemoryAddress == (IntPtr)GetNative().LayerB)
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

        public float FrequencyNoise { get { return GetNative().FrequencyNoise; } }
        public float AmplitudeNoise { get { return GetNative().AmplitudeNoise; } }
        public float Frequency { get { return GetNative().Frequency; } }
        public float Bias { get { return GetNative().Bias; } }

        public bool Disabled { get { return GetNative().Disabled; } }

        internal PostFxAnimationLayerBlend(PostFxAnimation anim)
        {
            animation = anim;
            memAddress = (IntPtr)Unsafe.AsPointer(ref animation.GetNative().FXStack.LayerBlend);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ref CAnimPostFX.LayerBlend GetNative() => ref Unsafe.AsRef<CAnimPostFX.LayerBlend>(memAddress.ToPointer());

        public bool IsValid()
        {
            return MemoryAddress != IntPtr.Zero;
        }
    }
}

