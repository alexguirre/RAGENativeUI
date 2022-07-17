namespace RAGENativeUI.Internals
{
    using Rage;

    using System;
    using System.Diagnostics;

    internal static unsafe class Grc
    {
        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(Grc)}] {str}");

        public static void BeginUsingDeviceContext() => ((delegate* unmanaged[Stdcall]<void>)Memory.grcBeginUsingDeviceContext)();
        public static void EndUsingDeviceContext() => ((delegate* unmanaged[Stdcall]<void>)Memory.grcEndUsingDeviceContext)();

        public static grcBufferFormat GetBufferFormatFromDXGIFormat(uint dxgiFormat) => ((delegate* unmanaged[Stdcall]<uint, grcBufferFormat>)Memory.grcGetBufferFormatFromDXGIFormat)(dxgiFormat);

        public static bool TryRegisterTextureDictionary(string name, out pgDictionary<grcTexture>* txd, out strLocalIndex index)
        {
            txd = null;
            index = default;

            ref var txdStore = ref fwTxdStore.Instance;

            if (txdStore.FindSlot(name).Value != 0xFFFFFFFF)
            {
                Log($"Failed to register texture dictionary '{name}': already exists");
                return false;
            }

            if (txdStore.NumUsedSlots == txdStore.Size)
            {
                Log($"Failed to register texture dictionary '{name}': store is full");
                return false;
            }

            txd = ConstructTextureDictionary();
            if (txd == null)
            {
                Log($"Failed to register texture dictionary '{name}': pgDictionary<grcTexture> construction failed"); 
                return false;
            }

            index = txdStore.Register(name);
            Log($"Registered texture dictionary '{name}' at index {index.Value}");
            txdStore.Set(index, txd);
            //rage::strStreaming::SetDoNotDefrag(&rage::strStreaming::ms_instance, v18.m_nIndex + rage::strStreaming::ms_instance.moduleMgr.modules[this->store->base.base.moduleIndex]->baseIndex);
            txdStore.AddRef(index);
            Log($" > NumRefs = {fwTxdStore.Instance.GetNumRefs(index)}");
            
            return true;
        }

        public static void RemoveTextureDictionary(string name, strLocalIndex index)
        {
            using var tls = UsingTls.Scope();

            Log($"Removing texture dictionary '{name}' at index {index}");
            Log($" > NumRefs Before RemoveRef = {fwTxdStore.Instance.GetNumRefs(index)}");
            fwTxdStore.Instance.RemoveRef(index);
            Log($" > NumRefs After RemoveRef  = {fwTxdStore.Instance.GetNumRefs(index)}");

            if (fwTxdStore.Instance.GetNumRefs(index) <= 0)
            {
                Log($"No references to texture dictionary '{name}', removing slot");
                fwTxdStore.Instance.RemoveSlot(index);
            }
        }

        public static pgDictionary<grcTexture>* ConstructTextureDictionary(int initialCapacity = 1)
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

        public static grcTexture* CreateTexture(uint width, uint height, TextureFormat format, IntPtr initialData, bool updatable)
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

        public static grcTexture* CreateTextureFromDDS(string name, byte[] ddsFile, bool updatable)
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

        public static void AddTexture(pgDictionary<grcTexture>* txd, string txdName, grcTexture* texture, string textureName)
        {
            if (txd->Find(textureName, out _))
            {
                throw new ArgumentException($"Texture dictionary '{txdName}' already contains '{textureName}'");
            }

            if (texture->Name != IntPtr.Zero)
            {
                sysMemAllocator.Current.Free(texture->Name);
            }

            texture->Name = Localization.ToUtf8(textureName);

            Log($"Adding texture '{textureName}' ({((IntPtr)texture).ToString("X")}) to texture dictionary '{txdName}'");
            Log($" > RefCount           = {texture->RefCount}");
            Log($" > UsageAndFlags      = {texture->UsageAndFlags:X2}");
            Log($" > Usage              = {texture->Usage}");
            Log($" > IsDynamic          = {texture->IsDynamic}");
            Log($" > HasPixelDataBuffer = {texture->HasPixelDataBuffer}");
            txd->Add(textureName, texture);
        }

        private static string MakeMemoryFileName(IntPtr address, uint size, bool freeOnClose, string name)
        {
            return $"memory:${address.ToString("X16")},{size},{(freeOnClose ? 1 : 0)}:{name}";
        }

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
