namespace RAGENativeUI
{
    using System;
    using System.Collections.Generic;

    using Rage;

    using RAGENativeUI.Memory;

    public unsafe struct TextureProperties
    {
        public string Name { get; }
        public int Width { get; }
        public int Height { get; }
        public int Depth { get; }
        public int Format { get; }

        internal TextureProperties(ref grcTexture texture)
        {
            Name = texture.GetName();
            Width = texture.width;
            Height = texture.height;
            Depth = texture.depth;
            Format = texture.format;
        }



        internal static TextureProperties GetFromDictionaryByName(TextureDictionary dictionary, uint nameHash)
        {
            int dictIndex = dictionary.Index;

            if (dictIndex != -1)
            {
                if (!dictionary.IsLoaded)
                {
                    dictionary.LoadAndWait();
                }

                ref pgDictionary<grcTexture> dict = ref GameMemory.TxdStore.Pool.Get(unchecked((uint)dictIndex)).TexturesDictionary;
                int index = dict.FindIndex(nameHash);

                if(index == -1)
                {
                    throw new InvalidOperationException($"The texture dictionary '{dictionary.Name}' doesn't contain the texture with hash 0x{nameHash:X8}.");
                }

                ref grcTexture texture = ref dict.Values[(short)index];
                TextureProperties t = new TextureProperties(ref texture);
                return t;
            }

            return default;
        }

        internal static TextureProperties GetFromDictionaryByIndex(TextureDictionary dictionary, int index)
        {
            int dictIndex = dictionary.Index;

            if (dictIndex != -1)
            {
                if (!dictionary.IsLoaded)
                {
                    dictionary.LoadAndWait();
                }

                ref pgDictionary<grcTexture> dict = ref GameMemory.TxdStore.Pool.Get(unchecked((uint)dictIndex)).TexturesDictionary;

                Throw.IfOutOfRange(index, 0, dict.Values.Count - 1, nameof(index));

                ref grcTexture texture = ref dict.Values[(short)index];
                TextureProperties t = new TextureProperties(ref texture);
                return t;
            }

            return default;
        }

        internal static IEnumerator<TextureProperties> GetEnumeratorForDictionary(TextureDictionary dictionary) => ((IEnumerable<TextureProperties>)GetAllFromDictionary(dictionary)).GetEnumerator();
        internal static TextureProperties[] GetAllFromDictionary(TextureDictionary dictionary)
        {
            int dictIndex = dictionary.Index;

            if (dictIndex != -1)
            {
                if (!dictionary.IsLoaded)
                {
                    dictionary.LoadAndWait();
                }

                ref pgDictionary<grcTexture> dict = ref GameMemory.TxdStore.Pool.Get(unchecked((uint)dictIndex)).TexturesDictionary;

                short count = dict.Values.Count;
                TextureProperties[] textures = new TextureProperties[count];
                for (short i = 0; i < count; i++)
                {
                    ref grcTexture texture = ref dict.Values[i];
                    TextureProperties t = new TextureProperties(ref texture);
                    textures[i] = t;
                }

                return textures;
            }

            return Array.Empty<TextureProperties>();
        }
    }
}

