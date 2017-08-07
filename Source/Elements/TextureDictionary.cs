namespace RAGENativeUI.Elements
{
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    public struct TextureDictionary : INamedAsset
    {
        public string Name { get; }
        public bool IsLoaded { get { return NativeFunction.Natives.HasStreamedTextureDictLoaded<bool>(Name); } }
        public string[] TextureNames { get { return GetTextureNames(this); } }

        public TextureDictionary(string name)
        {
            Name = name ?? throw new System.ArgumentNullException($"{nameof(Name)} can't be null.");
        }

        public void Dismiss()
        {
            NativeFunction.Natives.SetStreamedTextureDictAsNoLongerNeeded(Name);
        }

        public void Load()
        {
            NativeFunction.Natives.RequestStreamedTextureDict(Name, 1);
        }

        public void LoadAndWait()
        {
            Load();

            int endTime = System.Environment.TickCount + 5000;
            while (!IsLoaded && endTime > System.Environment.TickCount)
                GameFiber.Yield();
        }

        private static Dictionary<string, string[]> TextureNamesCache = new Dictionary<string, string[]>();
        private static unsafe string[] GetTextureNames(TextureDictionary dict)
        {
            if (TextureNamesCache.TryGetValue(dict.Name, out string[] n))
            {
                return n;
            }
            else
            {
                dict.LoadAndWait();

                if (!dict.IsLoaded)
                    return null;

                pgDictionary_grcTexture* txd = fwTxdStore.GetInstance()->GetDictionaryByName(dict.Name);
                if (txd != null)
                {
                    string[] names = new string[txd->Values.Count];
                    for (ushort i = 0; i < txd->Values.Count; i++)
                    {
                        grcTexture* texture = txd->Values.Get(i);
                        if (texture != null)
                        {
                            names[i] = texture->GetName();
                        }
                    }

                    TextureNamesCache[dict.Name] = names;
                    return names;
                }

                return null;
            }
        }

        public static implicit operator TextureDictionary(string name) => new TextureDictionary(name);
        public static implicit operator string(TextureDictionary textureDictionary) => textureDictionary.Name;
    }
}

