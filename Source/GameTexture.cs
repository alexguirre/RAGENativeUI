namespace RAGENativeUI
{
    using Rage;

    using RAGENativeUI.Internals;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public enum GameTextureFormat : uint
    {
        B8G8R8A8 = grcBufferFormat.B8G8R8A8_UNORM,
        R8G8B8A8 = grcBufferFormat.R8G8B8A8_UNORM,
    }

    public unsafe ref struct GameTextureMapData
    {
        internal grcMapData Native;

        public IntPtr Data => Native.Data;
        public uint Width => Native.Width;
        public uint Height => Native.Height;
        public uint Stride => Native.Stride;
        public uint BitsPerPixel => Native.BitsPerPixel;
    }

    public unsafe class GameTextureDictionary : IDisposable
    {
        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(GameTextureDictionary)}] {str}");

        private bool disposed;
        private readonly pgDictionary<grcTexture>* txd;
        private readonly strLocalIndex index;

        public string Name { get; }
        public string[] Textures
        {
            get
            {
                var textures = txd->Values;
                var names = new string[textures.Count];
                for (int i = 0; i < textures.Count; i++)
                {
                    names[i] = Localization.FromUtf8(textures[i]->Name);
                }
                return names;
            }
        }

        public GameTextureDictionary(string name)
        {
            Name = name;

            ref var txdStore = ref fwTxdStore.Instance;

            if (txdStore.FindSlot(name).Value != 0xFFFFFFFF)
            {
                throw new ArgumentException($"Texture dictionary '{name}' already exists");
            }

            if (txdStore.NumUsedSlots == txdStore.Size)
            {
                throw new InvalidOperationException("Texture dictionary is full");
            }

            txd = NewTxd();
            if (txd == null)
            {
                throw new InvalidOperationException("Failed to create texture dictionary");
            }

            index = txdStore.Register(name);
            Log($"Registered texture dictionary '{name}' at index {index.Value}");
            txdStore.Set(index, txd);
            //rage::strStreaming::SetDoNotDefrag(&rage::strStreaming::ms_instance, v18.m_nIndex + rage::strStreaming::ms_instance.moduleMgr.modules[this->store->base.base.moduleIndex]->baseIndex);
            txdStore.AddRef(index);
            Log($" > NumRefs = {fwTxdStore.Instance.GetNumRefs(index)}");
        }

        ~GameTextureDictionary()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                using var tls = UsingTls.Scope();

                Log($"Disposing texture dictionary '{Name}' at index {index.Value}");
                Log($" > NumRefs Before RemoveRef = {fwTxdStore.Instance.GetNumRefs(index)}");
                fwTxdStore.Instance.RemoveRef(index);
                Log($" > NumRefs After RemoveRef  = {fwTxdStore.Instance.GetNumRefs(index)}");

                if (fwTxdStore.Instance.GetNumRefs(index) <= 0)
                {
                    Log($"No references to texture dictionary '{Name}', removing slot");
                    fwTxdStore.Instance.RemoveSlot(index);
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool MapTexture(string name, out GameTextureMapData mapData)
        {
            mapData = default;
            if (!txd->Find(name, out var texture))
            {
                return false;
            }

            return texture->Map(0, 0, out mapData.Native, (grcTexture.MapFlags)3);
        }

        public void UnmapTexture(string name, in GameTextureMapData mapData)
        {
            if (!txd->Find(name, out var texture))
            {
                return;
            }

            texture->Unmap(mapData.Native);
        }

        // TODO: add option for creating texture with CPU write access
        public void AddTexture(string name, uint width, uint height, GameTextureFormat format, byte[] initialData)
        {
            fixed (byte* data = initialData)
            {
                AddTexture(name, width, height, format, (IntPtr)data);
            }
        }

        public void AddTexture(string name, uint width, uint height, GameTextureFormat format, IntPtr initialData)
        {
            var tex = NewTexture(width, height, format, initialData);
            AddTexture(name, tex);
        }

        public void AddTextureFromDDS(string name, string ddsFilePath)
        {
            Log($"Loading texture from '{ddsFilePath}'");
            AddTextureFromDDS(name, File.ReadAllBytes(ddsFilePath));
        }

        public void AddTextureFromDDS(string name, byte[] ddsFile)
        {
            var tex = NewTextureFromDDS(name, ddsFile);
            AddTexture(name, tex);
        }

        private void AddTexture(string name, grcTexture* texture)
        {
            if (texture->Name != IntPtr.Zero)
            {
                sysMemAllocator.Current.Free(texture->Name);
            }

            texture->Name = Localization.ToUtf8(name);

            Log($"Adding texture '{name}' ({((IntPtr)texture).ToString("X")}) to texture dictionary '{Name}'");
            txd->Add(name, texture);
        }

        private static pgDictionary<grcTexture>* NewTxd(int initialCapacity = 1)
        {
            if (Memory.pgDictionary_grcTexture_ctor == IntPtr.Zero)
            {
                Log("pgDictionary<grcTexture> constructor not available!");
                return null;
            }

            var txd = (pgDictionary<grcTexture>*)sysMemAllocator.Current.Allocate((ulong)sizeof(pgDictionary<grcTexture>), 0x10, 0);
            if (txd != null)
            {
                using var tls = UsingTls.Scope();

                var ctor = (delegate* unmanaged[Thiscall]<pgDictionary<grcTexture>*, int, pgDictionary<grcTexture>*>)Memory.pgDictionary_grcTexture_ctor;
                ctor(txd, initialCapacity);
                Log($"Created texture dictionary at {((IntPtr)txd).ToString("X")}");
            }

            return txd;
        }

        private static grcTexture* NewTexture(uint width, uint height, GameTextureFormat format, IntPtr initialData)
        {
            if (!grcTextureFactory.Available)
            {
                Log("grcTextureFactory not available!");
                return null;
            }

            using var tls = UsingTls.Scope();
            Log($"Creating texture from raw data (width:{width}, height:{height}, format:{format}, initialData:{initialData.ToString("X")})");
            return grcTextureFactory.Instance.Create(width, height, (grcBufferFormat)format, initialData);
        }

        private static grcTexture* NewTextureFromDDS(string name, byte[] ddsFile)
        {
            if (!grcTextureFactory.Available)
            {
                Log("grcTextureFactory not available!");
                return null;
            }

            fixed (byte* ptr = ddsFile)
            {
                using var tls = UsingTls.Scope();

                var memFileName = MakeMemoryFileName((IntPtr)ptr, (uint)ddsFile.Length, false, name);
                Log($"Creating texture from '{memFileName}'");
                return grcTextureFactory.Instance.Create(memFileName);
            }
        }

        private static string MakeMemoryFileName(IntPtr address, uint size, bool freeOnClose, string name)
        {
            return $"memory:${address.ToString("X16")},{size},{(freeOnClose ? 1 : 0)}:{name}";
        }
    }
}
