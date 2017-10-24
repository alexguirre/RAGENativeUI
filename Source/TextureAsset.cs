namespace RAGENativeUI
{
    using System;
    using System.Linq;
    using System.Drawing;
    using System.Collections.Generic;

    using Rage;

    using RAGENativeUI.Memory;

    public unsafe class TextureAsset : IAddressable
    {
        private readonly TextureDictionary dictionary;
        private readonly string name;
        private readonly int index;

        public TextureDictionary Dictionary => dictionary;
        public int Index => index;
        public string Name => name;
        public bool IsLoaded => Dictionary.IsLoaded;
        public IntPtr MemoryAddress => (IntPtr)GetNative();
        public SizeF Resolution
        {
            get
            {
                grcTexture* texture = GetNative();
                if (texture != null)
                {
                    return new SizeF(texture->width, texture->height);
                }

                return SizeF.Empty;
            }
        }
        public int Depth
        {
            get
            {
                grcTexture* texture = GetNative();
                if (texture != null)
                {
                    return texture->depth;
                }

                return 0;
            }
        }
        public int Format
        {
            get
            {
                grcTexture* texture = GetNative();
                if (texture != null)
                {
                    return texture->format;
                }

                return 0;
            }
        }

        internal TextureAsset(TextureDictionary dictionary, string name, int index)
        {
            this.dictionary = dictionary;
            this.name = name;
            this.index = index;
        }

        private grcTexture* GetNative()
        {
            grcTexture.pgDictionary* dict = dictionary.GetNative();

            if (dict == null)
                return null;

            return dict->Values.Get((short)index);
        }



        private static readonly TextureAsset[] emptyArray = new TextureAsset[0];
        private static readonly Dictionary<uint, Dictionary<uint, TextureAsset>> cache = new Dictionary<uint, Dictionary<uint, TextureAsset>>();
        
        internal static TextureAsset GetFromDictionary(TextureDictionary dictionary, uint nameHash)
        {
            uint dictHash = Game.GetHashKey(dictionary.Name);

            if (cache.TryGetValue(dictHash, out Dictionary<uint, TextureAsset> internalCacheDict) &&
                internalCacheDict.TryGetValue(nameHash, out TextureAsset cachedTexture))
            {
                return cachedTexture;
            }

            fwTxdStore* txdStore = GameMemory.TxdStore;
            uint dictIndex = txdStore->GetDictionaryPoolIndexByHash(dictHash);

            if(dictIndex != 0xFFFFFFFF)
            {
                if(!dictionary.IsLoaded)
                {
                    dictionary.LoadAndWait();
                }

                grcTexture.pgDictionary* dict = ((fwTxdDef*)txdStore->Pool.Get(dictIndex))->TexturesDictionary;
                int index = dict->GetValueIndex(nameHash);
                string name = dict->Values.Get((short)index)->GetName();
                TextureAsset t = new TextureAsset(dictionary, name, index);

                if (!cache.TryGetValue(dictHash, out Dictionary<uint, TextureAsset> internalCache))
                    cache[dictHash] = internalCache = new Dictionary<uint, TextureAsset>();

                internalCache[nameHash] = t;
                return t;
            }

            return null;
        }

        internal static IEnumerator<TextureAsset> GetEnumeratorForDictionary(TextureDictionary dictionary) => ((IEnumerable<TextureAsset>)GetAllFromDictionary(dictionary)).GetEnumerator();
        internal static TextureAsset[] GetAllFromDictionary(TextureDictionary dictionary)
        {
            uint dictHash = Game.GetHashKey(dictionary.Name);
            fwTxdStore* txdStore = GameMemory.TxdStore;
            uint dictIndex = txdStore->GetDictionaryPoolIndexByHash(dictHash);

            if (dictIndex != 0xFFFFFFFF)
            {
                if (!dictionary.IsLoaded)
                {
                    dictionary.LoadAndWait();
                }

                grcTexture.pgDictionary* dict = ((fwTxdDef*)txdStore->Pool.Get(dictIndex))->TexturesDictionary;

                if (cache.TryGetValue(dictHash, out Dictionary<uint, TextureAsset> internalCacheDict))
                {
                    if (dict->Values.Count == internalCacheDict.Count)
                    {
                        return internalCacheDict.Values.ToArray();
                    }
                }
                else
                {
                    cache[dictHash] = internalCacheDict = new Dictionary<uint, TextureAsset>();
                }

                short count = dict->Values.Count;
                TextureAsset[] textures = new TextureAsset[count]; 
                for (short i = 0; i < count; i++)
                {
                    uint nameHash = dict->Keys.Get(i);

                    if (!internalCacheDict.TryGetValue(nameHash, out TextureAsset t))
                    {
                        string name = dict->Values.Get(i)->GetName();
                        t = new TextureAsset(dictionary, name, i);
                        internalCacheDict[nameHash] = t;
                    }

                    textures[i] = t;
                }

                return textures;
            }

            return emptyArray;
        }
    }
}

