namespace RAGENativeUI
{
    using Rage;

    using RAGENativeUI.Internals;

    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public enum TextureFormat : uint
    {
        Unknown = grcBufferFormat.INVALID,

        R8 = grcBufferFormat.R8_UNORM,
        //R8G8 = grcBufferFormat.R8G8_UNORM, // NOTE: GetBufferFormatFromDXGIFormat doesn't round trip this format, so TextureLock.Format would return Unknown instead of R8G8, so disabled for now
        B8G8R8A8 = grcBufferFormat.B8G8R8A8_UNORM,
        R8G8B8A8 = grcBufferFormat.R8G8B8A8_UNORM,
    }

    public unsafe class TextureLock : IDisposable
    {
        private readonly grcMapData native;
        private bool disposed = false;

        public RuntimeTextureDictionary TextureDictionary { get; }
        public string TextureName { get; }
        public IntPtr Data => native.Data;
        public int Width => (int)native.Width;
        public int Height => (int)native.Height;
        public int Stride => (int)native.Stride;
        public int BitsPerPixel => (int)native.BitsPerPixel;
        public TextureFormat Format => (TextureFormat)Grc.GetBufferFormatFromDXGIFormat(native.Format);

        internal TextureLock(RuntimeTextureDictionary txd, string textureName, grcMapData native)
        {
            this.native = native;
            TextureDictionary = txd;
            TextureName = textureName;
        }

        ~TextureLock()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                TextureDictionary.UnmapTexture(TextureName, native);
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    public unsafe class RuntimeTextureDictionary : IDisposable
    {
        [Conditional("DEBUG")]
        private static void Log(string str) => Game.LogTrivialDebug($"[RAGENativeUI::{nameof(RuntimeTextureDictionary)}] {str}");

        private bool disposed;
        private readonly pgDictionary<grcTexture>* txd;
        private readonly strLocalIndex index;

        public string Name { get; }
        public string[] Textures
        {
            get
            {
                ThrowIfDisposed();

                var textures = txd->Values;
                var names = new string[textures.Count];
                for (int i = 0; i < textures.Count; i++)
                {
                    names[i] = Localization.FromUtf8(textures[i]->Name);
                }
                return names;
            }
        }

        private RuntimeTextureDictionary(string name, pgDictionary<grcTexture>* txd, strLocalIndex index)
        {
            Name = name;
            this.txd = txd;
            this.index = index;
        }

        ~RuntimeTextureDictionary()
        {
            Dispose(disposing: false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                Grc.ReleaseTxd(Name, index);
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool HasTexture(string name) => txd->Find(name, out _);

        public TextureLock MapTexture(string name)
        {
            ThrowIfDisposed();
            var texture = GetTextureOrThrow(name);

            Grc.BeginUsingDeviceContext();
            _ = texture->Map(0, 0, out var nativeMapData, (grcTexture.MapFlags)3);
            Grc.EndUsingDeviceContext();
            return new(this, name, nativeMapData);
        }

        internal void UnmapTexture(string name, in grcMapData mapData)
        {
            ThrowIfDisposed();
            var texture = GetTextureOrThrow(name);

            Grc.BeginUsingDeviceContext();
            texture->Unmap(mapData);
            Grc.EndUsingDeviceContext();
        }

        public void AddTexture(string name, int width, int height, TextureFormat format, byte[] initialData = null, bool updatable = false)
        {
            ThrowIfDisposed();
            ThrowIfInvalidNewTextureName(name);

            if (initialData != null)
            {
                fixed (byte* data = initialData)
                {
                    AddTexture(name, width, height, format, (IntPtr)data, updatable);
                }
            }
            else
            {
                AddTexture(name, width, height, format, IntPtr.Zero, updatable);
            }
        }

        public void AddTexture(string name, int width, int height, TextureFormat format, IntPtr initialData = default, bool updatable = false)
        {
            ThrowIfDisposed();
            ThrowIfInvalidNewTextureName(name);

            // TODO: check maximum size?
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Width must be greater than 0.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Height must be greater than 0.");
            }

            var tex = Grc.CreateTexture((uint)width, (uint)height, format, initialData, updatable);
            Grc.AddTexture(txd, Name, tex, name);
        }

        public void AddTextureFromDDS(string name, string ddsFilePath, bool updatable = false)
        {
            ThrowIfDisposed();
            ThrowIfInvalidNewTextureName(name);

            Log($"Loading texture from '{ddsFilePath}'");
            AddTextureFromDDS(name, File.ReadAllBytes(ddsFilePath), updatable);
        }

        public void AddTextureFromDDS(string name, byte[] ddsFile, bool updatable = false)
        {
            ThrowIfDisposed();
            ThrowIfInvalidNewTextureName(name);

            var tex = Grc.CreateTextureFromDDS(name, ddsFile, updatable);
            Grc.AddTexture(txd, Name, tex, name);
        }

        public void AddTextureFromImage(string name, Image image, bool updatable = false)
        {
            ThrowIfDisposed();
            ThrowIfInvalidNewTextureName(name);

            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var bitmap = image as Bitmap ?? new Bitmap(image); // Bitmap provides LockBits that allows accessing the raw data
            try
            {
                var (bitmapFormat, textureFormat) = image.PixelFormat switch
                {
                    // TODO: support more formats?
                    _ => (PixelFormat.Format32bppArgb, TextureFormat.B8G8R8A8),
                };

                var bitmapData = bitmap.LockBits(new Rectangle(new(0, 0), bitmap.Size), ImageLockMode.ReadOnly, bitmapFormat);
                AddTexture(name, bitmapData.Width, bitmapData.Height, textureFormat, bitmapData.Scan0, updatable);
                bitmap.UnlockBits(bitmapData);
            }
            finally
            {
                if (!ReferenceEquals(bitmap, image)) // dispose the bitmap copy if it was created
                {
                    bitmap.Dispose();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException($"Texture dictionary '{Name}'");
            }
        }

        private void ThrowIfInvalidNewTextureName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Texture name cannot be empty", nameof(name));
            }

            if (txd->Find(name, out _))
            {
                throw new ArgumentException($"Texture name '{name}' already exists in '{Name}'", nameof(name));
            }
        }

        private grcTexture* GetTextureOrThrow(string name)
        {
            if (!txd->Find(name, out var texture))
            {
                throw new ArgumentException($"Texture name '{name}' not found in '{Name}'", nameof(name));
            }

            return texture;
        }

        public static bool Create(string name, out RuntimeTextureDictionary runtimeTextureDictionary)
        {
            if (Grc.CreateRuntimeTxd(name, out var txd, out var index))
            {
                runtimeTextureDictionary = new(name, txd, index);
                return true;
            }
            else
            {
                runtimeTextureDictionary = null;
                return false;
            }
        }

        public static bool Get(string name, out RuntimeTextureDictionary runtimeTextureDictionary)
        {
            if (Grc.GetRuntimeTxd(name, out var txd, out var index))
            {
                runtimeTextureDictionary = new(name, txd, index);
                return true;
            }
            else
            {
                runtimeTextureDictionary = null;
                return false;
            }
        }

        public static bool GetOrCreate(string name, out RuntimeTextureDictionary runtimeTextureDictionary)
            => Get(name, out runtimeTextureDictionary) || Create(name, out runtimeTextureDictionary);
    }
}
