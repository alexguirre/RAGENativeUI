namespace RAGENativeUI
{
    using System.Runtime.CompilerServices;
    using System.Collections.Generic;

    internal static class Cache
    {
        private static readonly Dictionary<uint, TimeCycleModifier> timeCycleModifiers = new Dictionary<uint, TimeCycleModifier>();
        private static readonly Dictionary<uint, PostFxAnimation> postFxAnimations = new Dictionary<uint, PostFxAnimation>();
        private static readonly Dictionary<uint, CustomTexture> customTextures = new Dictionary<uint, CustomTexture>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Get<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, out TValue value)
        {
            if (dictionary.TryGetValue(key, out TValue v))
            {
                value = v;
                return true;
            }

            value = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Add<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            dictionary[key] = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Remove<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key)
        {
            dictionary.Remove(key);
        }

        public static bool Get(uint hash, out TimeCycleModifier timeCycleModifier) => Get(timeCycleModifiers, hash, out timeCycleModifier);
        public static bool Get(uint hash, out PostFxAnimation postFxAnimation) => Get(postFxAnimations, hash, out postFxAnimation);
        public static bool Get(uint hash, out CustomTexture customTexture) => Get(customTextures, hash, out customTexture);

        public static void Add(TimeCycleModifier timeCycleModifier) => Add(timeCycleModifiers, timeCycleModifier.Hash, timeCycleModifier);
        public static void Add(PostFxAnimation postFxAnimation) => Add(postFxAnimations, postFxAnimation.Hash, postFxAnimation);
        public static void Add(CustomTexture customTexture) => Add(customTextures, customTexture.Hash, customTexture);

        public static void Remove(TimeCycleModifier timeCycleModifier) => Remove(timeCycleModifiers, timeCycleModifier.Hash);
        public static void Remove(PostFxAnimation postFxAnimation) => Remove(postFxAnimations, postFxAnimation.Hash);
        public static void Remove(CustomTexture customTexture) => Remove(customTextures, customTexture.Hash);

    }
}

