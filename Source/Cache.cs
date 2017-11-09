namespace RAGENativeUI
{
    using System.Runtime.CompilerServices;
    using System.Collections.Generic;

    internal static class Cache
    {
        private static Dictionary<uint, TimeCycleModifier> timeCycleModifiers = new Dictionary<uint, TimeCycleModifier>();
        private static Dictionary<uint, PostFxAnimation> postFxAnimations = new Dictionary<uint, PostFxAnimation>();

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

        public static bool Get(uint hash, out TimeCycleModifier timeCycleModifier) => Get(timeCycleModifiers, hash, out timeCycleModifier);
        public static bool Get(uint hash, out PostFxAnimation postFxAnimation) => Get(postFxAnimations, hash, out postFxAnimation);

        public static void Add(TimeCycleModifier timeCycleModifier) => Add(timeCycleModifiers, timeCycleModifier.Hash, timeCycleModifier);
        public static void Add(PostFxAnimation postFxAnimation) => Add(postFxAnimations, postFxAnimation.Hash, postFxAnimation);
    }
}

