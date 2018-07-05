namespace RAGENativeUI
{
    using System;
    using System.Runtime.InteropServices;

    using Rage;

    public unsafe struct TextureDictionary : INamedAsset, IValidatable
    {
        public string Name { get; }
        public bool IsLoaded => N.HasStreamedTextureDictLoaded(Name);
        public bool IsValid => RNUI.Helper.DoesTextureDictionaryExist(Name);

        public TextureDictionary(string name)
        {
            Throw.IfNull(name, nameof(name));

            Name = name;
        }

        public void Dismiss()
        {
            N.SetStreamedTextureDictAsNoLongerNeeded(Name);
        }

        public void Load()
        {
            N.RequestStreamedTextureDict(Name, true);
        }

        public void LoadAndWait()
        {
            if (IsValid)
            {
                Load();

                int endTime = Environment.TickCount + 5000;
                while (!IsLoaded && endTime > Environment.TickCount)
                    GameFiber.Yield();
            }
        }

        public TextureReference[] GetTextures()
        {
            if (!IsValid)
            {
                return Array.Empty<TextureReference>();
            }

            LoadAndWait();

            uint c = RNUI.Helper.GetNumberOfTexturesFromDictionary(Name);

            if (c != 0xFFFFFFFF && c > 0)
            {
                RNUI.Helper.TextureDesc[] textureDescs = new RNUI.Helper.TextureDesc[c];
                RNUI.Helper.GetTexturesFromDictionary(Name, textureDescs);
                string dictName = Name;
                return Array.ConvertAll(textureDescs, (t) => { return new TextureReference(dictName, Marshal.PtrToStringAnsi(t.Name), (int)t.Width, (int)t.Height); });
            }

            return Array.Empty<TextureReference>();
        }

        public static implicit operator TextureDictionary(string name) => new TextureDictionary(name);
        public static implicit operator string(TextureDictionary textureDictionary) => textureDictionary.Name;
    }
}

