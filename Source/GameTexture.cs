namespace RAGENativeUI
{
    using Rage;

    using RAGENativeUI.Internals;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    public enum GameTextureFormat : uint
    {
        R8 = grcBufferFormat.R8_UNORM,
        R8G8 = grcBufferFormat.R8G8_UNORM,
        B8G8R8A8 = grcBufferFormat.B8G8R8A8_UNORM,
        R8G8B8A8 = grcBufferFormat.R8G8B8A8_UNORM,
    }

    public unsafe readonly ref struct GameTextureMapData
    {
        internal readonly grcMapData Native;

        internal GameTextureMapData(grcMapData native) => Native = native;

        public IntPtr Data => Native.Data;
        public uint Width => Native.Width;
        public uint Height => Native.Height;
        public uint Stride => Native.Stride;
        public uint BitsPerPixel => Native.BitsPerPixel;
        public uint DXGIFormat => Native.Format;
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

        public grcTexture* GetTexture(string name)
        {
            if (txd->Find(name, out var texture))
            {
                return texture;
            }

            return null;
        }

        public bool MapTexture(string name, out GameTextureMapData mapData)
        {
            if (!txd->Find(name, out var texture))
            {
                mapData = default;
                return false;
            }

            BeginUsingDeviceContext();
            var result = texture->Map(0, 0, out var nativeMapData, (grcTexture.MapFlags)3);
            EndUsingDeviceContext();
            mapData = new(nativeMapData);
            return result;
        }

        public void UnmapTexture(string name, in GameTextureMapData mapData)
        {
            if (!txd->Find(name, out var texture))
            {
                return;
            }

            BeginUsingDeviceContext();
            texture->Unmap(mapData.Native);
            EndUsingDeviceContext();
        }

        public void AddTexture(string name, uint width, uint height, GameTextureFormat format, byte[] initialData, bool updatable = false)
        {
            fixed (byte* data = initialData)
            {
                AddTexture(name, width, height, format, (IntPtr)data, updatable);
            }
        }

        public void AddTexture(string name, uint width, uint height, GameTextureFormat format, IntPtr initialData, bool updatable = false)
        {
            var tex = NewTexture(width, height, format, initialData, updatable);
            AddTexture(name, tex);
        }

        public void AddTextureFromDDS(string name, string ddsFilePath, bool updatable = false)
        {
            Log($"Loading texture from '{ddsFilePath}'");
            AddTextureFromDDS(name, File.ReadAllBytes(ddsFilePath), updatable);
        }

        public void AddTextureFromDDS(string name, byte[] ddsFile, bool updatable = false)
        {
            var tex = NewTextureFromDDS(name, ddsFile, updatable);
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
            Log($" > RefCount           = {texture->RefCount}");
            Log($" > UsageAndFlags      = {texture->UsageAndFlags:X2}");
            Log($" > Usage              = {texture->Usage}");
            Log($" > IsDynamic          = {texture->IsDynamic}");
            Log($" > HasPixelDataBuffer = {texture->HasPixelDataBuffer}");
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

        private static grcTexture* NewTexture(uint width, uint height, GameTextureFormat format, IntPtr initialData, bool updatable)
        {
            if (!grcTextureFactory.Available)
            {
                Log("grcTextureFactory not available!");
                return null;
            }

            using var tls = UsingTls.Scope();

            Log($"Creating texture from raw data (width:{width}, height:{height}, format:{format}, initialData:{initialData.ToString("X")}, updatable:{updatable})");
            var createParams = UpdatableTextureCreateParams;
            return grcTextureFactory.Instance.Create(width, height, (grcBufferFormat)format, initialData, updatable ? &createParams : null);
        }

        private static grcTexture* NewTextureFromDDS(string name, byte[] ddsFile, bool updatable)
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
                Log($"Creating texture from DDS  (memFileName:{memFileName}, updatable:{updatable})");
                var createParams = UpdatableTextureCreateParams;
                return grcTextureFactory.Instance.Create(memFileName, updatable ? &createParams : null);
            }
        }

        private static string MakeMemoryFileName(IntPtr address, uint size, bool freeOnClose, string name)
        {
            return $"memory:${address.ToString("X16")},{size},{(freeOnClose ? 1 : 0)}:{name}";
        }

        private static void BeginUsingDeviceContext() => ((delegate* unmanaged[Stdcall]<void>)Memory.grcBeginUsingDeviceContext)();
        private static void EndUsingDeviceContext() => ((delegate* unmanaged[Stdcall]<void>)Memory.grcEndUsingDeviceContext)();

        /// <summary>
        /// These params create a texture that can be updated through a staging texture.
        /// </summary>
        private static readonly grcTextureFactory.TextureCreateParams UpdatableTextureCreateParams
            = new()
            {
                UsageVar1 = 0,
                field_4 = 0,
                UsageVar2 = 3,
                field_10 = 0,
                IsRenderTarget = 0,
                field_1C = 0,
                field_20 = 2,
                MipLevels = 1,
                field_28 = 0,
            };
    }
}
