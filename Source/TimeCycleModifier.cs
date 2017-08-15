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
    public unsafe sealed class TimeCycleModifier : IAddressable
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
                return *GameMemory.CurrentTimeCycleModifierIndex == index;
            }
            set
            {
                if (value)
                {
                    *GameMemory.CurrentTimeCycleModifierIndex = index;
                }
                else if (IsActive)
                {
                    *GameMemory.CurrentTimeCycleModifierIndex = -1;
                }
            }
        }

        public IntPtr MemoryAddress { get { return memAddress; } }
        public int Index { get { return index; } }

        private TimeCycleModifier(CTimeCycleModifier* native, int idx)
        {
            hash = native->Name;
            memAddress = (IntPtr)native;
            index = idx;
        }

        public bool IsValid()
        {
            return memAddress != IntPtr.Zero;
        }

        public static TimeCycleModifier GetByName(string name)
        {
            uint hash = Game.GetHashKey(name);
            knownNames[hash] = name;
            return GetByHash(hash);
        }

        public static TimeCycleModifier GetByHash(uint hash)
        {
            if (cache.TryGetValue(hash, out TimeCycleModifier p))
            {
                return p;
            }
            else
            {
                int index= GameFunctions.GetPostFXModifierIndex(GameMemory.TimeCycleModifiersManager, &hash);

                if (index != -1)
                {
                    return GetByIndex(index);
                }
            }

            return null;
        }

        public static TimeCycleModifier GetByIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            short i = (short)index;
            CTimeCycleModifier* native = GameMemory.TimeCycleModifiersManager->Modifiers.Get(i);

            if (native != null)
            {
                if (cache.TryGetValue(native->Name, out TimeCycleModifier p))
                {
                    return p;
                }
                else
                {
                    TimeCycleModifier m = new TimeCycleModifier(native, index);
                    cache[native->Name] = m;
                    return m;
                }
            }

            return null;
        }

        public static TimeCycleModifier[] GetAll()
        {
            TimeCycleModifier[] mods = new TimeCycleModifier[GameMemory.TimeCycleModifiersManager->Modifiers.Count];
            for (short i = 0; i < GameMemory.TimeCycleModifiersManager->Modifiers.Count; i++)
            {
                mods[i] = GetByIndex(i);
            }

            return mods;
        }

        public static int Count
        {
            get
            {
                return GameMemory.TimeCycleModifiersManager->Modifiers.Count;
            }
        }

        public static TimeCycleModifier CurrentModifier
        {
            get
            {
                int index = *GameMemory.CurrentTimeCycleModifierIndex;
                return index == -1 ? null : GetByIndex(index);
            }
        }

        private static Dictionary<uint, TimeCycleModifier> cache = new Dictionary<uint, TimeCycleModifier>();

        private static Dictionary<uint, string> knownNames = new Dictionary<uint, string>()
        {
        };
    }
}

