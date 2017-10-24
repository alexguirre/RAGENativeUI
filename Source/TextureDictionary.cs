namespace RAGENativeUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    public unsafe struct TextureDictionary : INamedAsset, IAddressable, IEnumerable<TextureAsset>
    {
        public string Name { get; }
        public bool IsValid => GameMemory.TxdStore->GetDictionaryPoolIndexByHash(Game.GetHashKey(Name)) != 0xFFFFFFFF;
        public bool IsLoaded => NativeFunction.Natives.HasStreamedTextureDictLoaded<bool>(Name);
        /// <summary>
        /// Gets the memory address of this instance. If this <see cref="TextureDictionary"/> isn't loaded, returns <see cref="IntPtr.Zero"/>.
        /// </summary>
        public IntPtr MemoryAddress => (IntPtr)GetNative();

        public TextureAsset this[string name] { get { Throw.IfNull(name, nameof(name)); return TextureAsset.GetFromDictionary(this, Game.GetHashKey(name)); } }
        public TextureAsset this[uint nameHash] => TextureAsset.GetFromDictionary(this, nameHash);

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
            if (!IsValid)
                return;

            Load();

            int endTime = Environment.TickCount + 5000;
            while (!IsLoaded && endTime > Environment.TickCount)
                GameFiber.Yield();
        }

        public bool Contains(string textureName) => Contains(Game.GetHashKey(textureName));
        public bool Contains(uint textureNameHash)
        {
            if (!IsLoaded)
            {
                LoadAndWait();
            }

            grcTexture.pgDictionary* dict = GetNative();
            int index = dict->GetValueIndex(textureNameHash);
            return index != -1;
        }

        internal grcTexture.pgDictionary* GetNative()
        {
            if (!IsLoaded)
                return null;

            return GameMemory.TxdStore->GetDictionaryByName(Name);
        }

        public TextureAsset[] ToArray() => TextureAsset.GetAllFromDictionary(this);

        public IEnumerator<TextureAsset> GetEnumerator() => TextureAsset.GetEnumeratorForDictionary(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator TextureDictionary(string name) => new TextureDictionary(name);
        public static implicit operator string(TextureDictionary textureDictionary) => textureDictionary.Name;
    }
}

