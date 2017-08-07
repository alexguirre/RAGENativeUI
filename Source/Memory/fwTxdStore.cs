namespace RAGENativeUI.Memory
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    [StructLayout(LayoutKind.Explicit)]
    internal unsafe struct fwTxdStore
    {
        [StructLayout(LayoutKind.Sequential, Size = 8)]
        public struct TxdNameHash
        {
            public uint Hash;
            public ushort PoolIndex;
            public ushort NextIndex;
        }

        [StructLayout(LayoutKind.Sequential, Size = 2)]
        public struct TxdIndex
        {
            public ushort HashIndex;
        }

        [FieldOffset(0x0038)] public CPool Pool;

        [FieldOffset(0x0070)] public TxdNameHash* HashesArray;
        [FieldOffset(0x0078)] public TxdIndex* IndicesArray;
        [FieldOffset(0x0080)] public ushort IndicesArraySize;
        [FieldOffset(0x0082)] public ushort HashesArraySize;

        public pgDictionary_grcTexture* GetDictionaryByName(string name) => GetDictionaryByHash(Game.GetHashKey(name));
        public pgDictionary_grcTexture* GetDictionaryByHash(uint hash)
        {
            ushort index = IndicesArray[hash % IndicesArraySize].HashIndex;

            if (index == 0xFFFF)
                return null;

            while (HashesArray[index].Hash != hash)
            {
                index = HashesArray[index].NextIndex;
                if (index == 0xFFFF)
                    return null;
            }

            uint poolIndex = HashesArray[index].PoolIndex;
            fwTxdDef* def = (fwTxdDef*)Pool.Get(poolIndex);
            return def == null ? null : def->TexturesDictionary;
        }

        private static fwTxdStore* instance;
        public static fwTxdStore* GetInstance()
        {
            if (instance == null)
            {
                IntPtr address = Game.FindPattern("48 8D 0D ?? ?? ?? ?? E8 ?? ?? ?? ?? 8B 45 EC 4C 8D 45 F0 48 8D 55 EC 48 8D 0D");
                if (address == IntPtr.Zero)
                {
                    Common.Log($"Incompatible game version, couldn't find {nameof(fwTxdStore)} instance.");
                    return null;
                }

                address = address + *(int*)(address + 3) + 7;
                instance = (fwTxdStore*)address;
            }

            return instance;
        }
    }
}

