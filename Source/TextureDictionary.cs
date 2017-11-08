namespace RAGENativeUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Memory;

    public unsafe struct TextureDictionary : INamedAsset, IAddressable, IReadOnlyList<TextureProperties>
    {
        private int index;

        public string Name { get; }
        public bool IsValid => GameMemory.TxdStore.GetDictionaryPoolIndexByHash(Game.GetHashKey(Name)) != 0xFFFFFFFF;
        public bool IsLoaded => NativeFunction.Natives.HasStreamedTextureDictLoaded<bool>(Name);
        /// <summary>
        /// Gets the memory address of this instance. If this <see cref="TextureDictionary"/> isn't loaded, returns <see cref="IntPtr.Zero"/>.
        /// </summary>
        public IntPtr MemoryAddress => (IntPtr)Unsafe.AsPointer(ref GetNative());

        public TextureProperties this[string name] { get { Throw.IfNull(name, nameof(name)); return TextureProperties.GetFromDictionaryByName(this, Game.GetHashKey(name)); } }
        public TextureProperties this[uint nameHash] => TextureProperties.GetFromDictionaryByName(this, nameHash);
        public TextureProperties this[int index] => TextureProperties.GetFromDictionaryByIndex(this, index);
        public int Count => GetCount();

        internal int Index
        {
            get
            {
                if(index == -1)
                {
                    uint hash = Game.GetHashKey(Name);
                    index = unchecked((int)GameMemory.TxdStore.GetDictionaryPoolIndexByHash(hash));
                }

                return index;
            }
        }

        public TextureDictionary(string name)
        {
            Throw.IfNull(name, nameof(name));

            Name = name;
            index = -1;
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
            
            return GetNative().ContainsKey(textureNameHash);
        }

        internal ref pgDictionary<grcTexture> GetNative()
        {
            if (!IsLoaded)
                throw new InvalidOperationException();

            return ref GameMemory.TxdStore.GetDictionaryByName(Name);
        }

        internal int GetCount()
        {
            uint dictHash = Game.GetHashKey(Name);
            ref fwTxdStore txdStore = ref GameMemory.TxdStore;
            uint dictIndex = txdStore.GetDictionaryPoolIndexByHash(dictHash);

            if (dictIndex != 0xFFFFFFFF)
            {
                if (!IsLoaded)
                {
                    LoadAndWait();
                }

                ref pgDictionary<grcTexture> dict = ref txdStore.Pool[dictIndex].TexturesDictionary.Ref;

                return dict.Values.Count;
            }

            return -1;
        }

        public TextureProperties[] ToArray() => TextureProperties.GetAllFromDictionary(this);

        public IEnumerator<TextureProperties> GetEnumerator() => TextureProperties.GetEnumeratorForDictionary(this);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static implicit operator TextureDictionary(string name) => new TextureDictionary(name);
        public static implicit operator string(TextureDictionary textureDictionary) => textureDictionary.Name;
    }
}

