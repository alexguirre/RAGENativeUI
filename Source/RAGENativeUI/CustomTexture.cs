namespace RAGENativeUI
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;

    public sealed class CustomTexture : TextureReference, IDisposable
#if !RPH1
        , Rage.IValidatable
#endif
    {
        internal uint Hash { get; }
        public bool IsUpdatable { get; }
        public bool IsValid => RNUI.Helper.DoesCustomTextureExist(Hash);

        internal CustomTexture(string name, int width, int height, bool updatable) : base(CustomTexturesDictionary, name, width, height)
        {
            Hash = RPH.Game.GetHashKey(name);
            IsUpdatable = updatable;

            Cache.Add(this);

            Common.LogDebug($"Created new custom texture: Name -> {name}, W -> {width}, H -> {height}, Updatable -> {updatable}");
        }

        ~CustomTexture()
        {
            Dispose(false);
        }

        #region IDisposable Support
        void Dispose(bool disposing)
        {
            if (IsValid)
            {
                if (disposing)
                {
                    Cache.Remove(this);
                }

                RNUI.Helper.DeleteCustomTexture(Hash);
                Common.LogDebug($"Deleted custom texture: Name -> {Name}");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        /// <summary>
        /// Gets the <see cref="TextureDictionary"/> that contains the created <see cref="CustomTexture"/>s.
        /// </summary>
        public static TextureDictionary CustomTexturesDictionary { get; } = "rnui_custom_textures";

        public static CustomTexture FromFile(string name, string filePath, bool updatable)
        {
            Throw.IfNull(filePath, nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException($"The file specified in '{nameof(filePath)}' was not found.", filePath);

            using (Bitmap bitmap = new Bitmap(filePath))
            {
                return FromBitmap(name, bitmap, updatable);
            }
        }

        public static CustomTexture FromBitmap(string name, Bitmap bitmap, bool updatable)
        {
            Throw.IfNull(bitmap, nameof(bitmap));
            Throw.InvalidOperationIfNot(bitmap.PixelFormat == PixelFormat.Format32bppArgb, "Invalid pixel format"); // TODO: support other formats

            byte[] dst = new byte[bitmap.Width * 4 * bitmap.Height];
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, dst, 0, dst.Length);

            bitmap.UnlockBits(bitmapData);

            return FromPixelData(name, bitmap.Width, bitmap.Height, dst, updatable);
        }

        public static unsafe CustomTexture FromPixelData(string name, int width, int height, byte[] pixelData, bool updatable)
        {
            Throw.IfNull(name, nameof(name));
            Throw.InvalidOperationIf(RNUI.Helper.DoesCustomTextureExist(RPH.Game.GetHashKey(name)), $"Custom texture with name '{name}' already exists.");
            Throw.IfNull(pixelData, nameof(pixelData));
            Throw.ArgumentExceptionIfNot((pixelData.Length == (width * 4 * height)));
            Throw.IfNegativeOrZero(width, nameof(width));
            Throw.IfNegativeOrZero(height, nameof(height));

            bool success = false;
            fixed (byte* data = pixelData)
            {
                success = RNUI.Helper.CreateCustomTexture(name, (uint)width, (uint)height, (IntPtr)data, updatable);
            }

            return success ? new CustomTexture(name, width, height, updatable) : null;
        }

        public static bool DoesCustomTextureExists(string name) => RNUI.Helper.DoesCustomTextureExist(RPH.Game.GetHashKey(name));
        public static uint GetNumberOfCustomTextures() => RNUI.Helper.GetNumberOfCustomTextures();
    }
}

