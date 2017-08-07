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
            public ushort Index;
        }

        [FieldOffset(0x0038)] public CPool Pool;

        [FieldOffset(0x0070)] public TxdNameHash* HashesArray;
        [FieldOffset(0x0082)] public ushort HashesArraySize;

        public fwTxdDef* GetPoolItem(int index)
        {
            return (fwTxdDef*)Pool.Get(unchecked((uint)index));
        }

        public pgDictionary_grcTexture* GetDictionaryByName(string name) => GetDictionaryByHash(Game.GetHashKey(name));
        public pgDictionary_grcTexture* GetDictionaryByHash(uint hash)
        {
            for (uint i = 0; i < HashesArraySize; i++)
            {
                TxdNameHash h = HashesArray[i];

                if (h.Hash != hash)
                    continue;

                ushort index = h.Index;

                if (index == 0xFFFF)
                    break;

                fwTxdDef* def = GetPoolItem(index);
                return def->TexturesDictionary;
            }

            return null;
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

