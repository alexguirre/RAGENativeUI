namespace RAGENativeUI
{
    using Rage;

    public readonly struct TextureDictionary : INamedAsset
    {
        // TODO: should TextureDictionary check for "embed:"?

        public string Name { get; }

        public bool IsLoaded => N.HasStreamedTextureDictLoaded(Name);

        public TextureDictionary(string name) => Name = name;

        public void Dismiss() => N.SetStreamedTextureDictAsNoLongerNeeded(Name);
        public void Load() => N.RequestStreamedTextureDict(Name);
        public void LoadAndWait()
        {
            Load();
            var txd = this;
            GameFiber.SleepUntil(() => txd.IsLoaded, timeOut: 10_000);
        }

        public static implicit operator TextureDictionary(string textureDictionaryName) => new(textureDictionaryName);
        public static implicit operator string(TextureDictionary textureDictionary) => textureDictionary.Name;
    }
}
