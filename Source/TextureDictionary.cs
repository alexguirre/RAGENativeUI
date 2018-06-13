namespace RAGENativeUI
{
    using System;

    using Rage;

    public unsafe struct TextureDictionary : INamedAsset
    {
        public string Name { get; }
        public bool IsLoaded => N.HasStreamedTextureDictLoaded(Name);

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
            Load();

            int endTime = Environment.TickCount + 5000;
            while (!IsLoaded && endTime > Environment.TickCount)
                GameFiber.Yield();
        }

        public static implicit operator TextureDictionary(string name) => new TextureDictionary(name);
        public static implicit operator string(TextureDictionary textureDictionary) => textureDictionary.Name;
    }
}

