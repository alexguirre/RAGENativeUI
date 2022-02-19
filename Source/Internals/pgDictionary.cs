namespace RAGENativeUI.Internals
{
    using Rage;

    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Size = 0x40)]
    internal unsafe struct pgDictionary<T> where T : unmanaged
    {
        private fixed byte padding[0x20];
        public atArray<uint> Keys;
        public atArrayOfPtrs<T> Values;

        public bool Add(string key, T* value) => Add(Game.GetHashKey(key), value);
        public bool Add(uint key, T* value)
        {
            int insertIndex = Keys.Count;
            if (Keys.Count > 0)
            {
                for (int i = 0; i < Keys.Count; i++)
                {
                    if (Keys[i] == key)
                    {
                        // key already exists
                        return false;
                    }

                    if (Keys[i] > key)
                    {
                        insertIndex = i;
                        break;
                    }
                }
            }

            // expand arrays
            Keys.Add();
            Values.Add();

            // shift keys/values after the insert position
            for (int i = Keys.Count - 1; i > insertIndex; --i)
            {
                Keys[i] = Keys[i - 1];
                Values[i] = Values[i - 1];
            }

            // insert the key/value pair
            Keys[insertIndex] = key;
            Values[insertIndex] = value;
            return true;
        }

        public bool Find(string key, out T* value) => Find(Game.GetHashKey(key), out value);
        public bool Find(uint key, out T* value)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i] == key)
                {
                    value = Values[i];
                    return true;
                }

                if (Keys[i] > key)
                {
                    break;
                }
            }

            // not found
            value = null;
            return false;
        }
    }
}
