namespace RAGENativeUI
{
    using System;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    public struct TextureDictionary : INamedAsset, IAddressable
    {
        public string Name { get; }
        public bool IsLoaded { get { return NativeFunction.Natives.HasStreamedTextureDictLoaded<bool>(Name); } }
        public string[] Textures { get { return GetTextureNames(this); } }
        /// <summary>
        /// Gets the memory address of this instance. If this <see cref="TextureDictionary"/> isn't loaded, returns <see cref="IntPtr.Zero"/>.
        /// </summary>
        public unsafe IntPtr MemoryAddress { get { return IsLoaded ? (IntPtr)GameMemory.TxdStore->GetDictionaryByName(Name) : IntPtr.Zero; } }

        public TextureDictionary(string name)
        {
            Throw.IfNull(name, nameof(name));

            Name = name;
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

            int endTime = Environment.TickCount + 5000;
            while (!IsLoaded && endTime > Environment.TickCount)
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

                grcTexture.pgDictionary* txd = GameMemory.TxdStore->GetDictionaryByName(dict.Name);
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

