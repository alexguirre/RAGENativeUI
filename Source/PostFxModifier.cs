namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    // defined in timecycle_mods_x.xml
    public unsafe sealed class PostFxModifier : IAddressable
    {
        private uint hash;
        private IntPtr memAddress;
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
                return *GameMemory.CurrentPostFXModifierIndex == index;
            }
            set
            {
                *GameMemory.CurrentPostFXModifierIndex = value ? index : -1;
            }
        }

        public IntPtr MemoryAddress { get { return memAddress; } }
        public int Index { get { return index; } }

        private PostFxModifier(CPostFXModifier* native, int idx)
        {
            hash = native->Name;
            memAddress = (IntPtr)native;
            index = idx;
        }

        public bool IsValid()
        {
            return memAddress != IntPtr.Zero;
        }

        public static PostFxModifier GetByName(string name)
        {
            uint hash = Game.GetHashKey(name);
            knownNames[hash] = name;
            return GetByHash(hash);
        }

        public static PostFxModifier GetByHash(uint hash)
        {
            if (cache.TryGetValue(hash, out PostFxModifier p))
            {
                return p;
            }
            else
            {
                int index= GameFunctions.GetPostFXModifierIndex(GameMemory.PostFXModifiersManager, &hash);

                if (index != -1)
                {
                    return GetByIndex(index);
                }
            }

            return null;
        }

        public static PostFxModifier GetByIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            short i = (short)index;
            CPostFXModifier* native = GameMemory.PostFXModifiersManager->Modifiers.Get(i);

            if (native != null)
            {
                if (cache.TryGetValue(native->Name, out PostFxModifier p))
                {
                    return p;
                }
                else
                {
                    PostFxModifier m = new PostFxModifier(native, index);
                    cache[native->Name] = m;
                    return m;
                }
            }

            return null;
        }

        public static PostFxModifier[] GetAll()
        {
            PostFxModifier[] mods = new PostFxModifier[GameMemory.PostFXModifiersManager->Modifiers.Count];
            for (short i = 0; i < GameMemory.PostFXModifiersManager->Modifiers.Count; i++)
            {
                mods[i] = GetByIndex(i);
            }

            return mods;
        }

        public static int Count
        {
            get
            {
                return GameMemory.PostFXModifiersManager->Modifiers.Count;
            }
        }

        public static PostFxModifier CurrentModifier
        {
            get
            {
                int index = *GameMemory.CurrentPostFXModifierIndex;
                return index == -1 ? null : GetByIndex(index);
            }
        }

        private static Dictionary<uint, PostFxModifier> cache = new Dictionary<uint, PostFxModifier>();

        private static Dictionary<uint, string> knownNames = new Dictionary<uint, string>()
        {
        };
    }
}

