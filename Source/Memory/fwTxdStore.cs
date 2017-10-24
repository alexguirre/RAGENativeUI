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

        public grcTexture.pgDictionary* GetDictionaryByName(string name) => GetDictionaryByHash(Game.GetHashKey(name));
        public grcTexture.pgDictionary* GetDictionaryByHash(uint hash)
        {
            uint poolIndex = GetDictionaryPoolIndexByHash(hash);
            if (poolIndex != 0xFFFFFFFF)
            {
                fwTxdDef* def = (fwTxdDef*)Pool.Get(poolIndex);
                return def == null ? null : def->TexturesDictionary;
            }

            return null;
        }

        public uint GetDictionaryPoolIndexByHash(uint hash)
        {
            ushort index = IndicesArray[hash % IndicesArraySize].HashIndex;

            if (index == 0xFFFF)
                return 0xFFFFFFFF;

            while (HashesArray[index].Hash != hash)
            {
                index = HashesArray[index].NextIndex;
                if (index == 0xFFFF)
                    return 0xFFFFFFFF;
            }

            return HashesArray[index].PoolIndex;
        }
    }
}

