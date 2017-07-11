namespace RAGENativeUI.Elements
{
    using Rage;
    using Rage.Native;

    public struct TextureDictionary : INamedAsset
    {
        public string Name { get; }
        public bool IsLoaded { get { return NativeFunction.Natives.HasStreamedTextureDictLoaded<bool>(Name); } }

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
                GameFiber.Sleep(10);
        }

        public static implicit operator TextureDictionary(string name) => new TextureDictionary(name);
        public static implicit operator string(TextureDictionary textureDictionary) => textureDictionary.Name;
    }
}

