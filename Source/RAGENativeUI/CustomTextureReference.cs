namespace RAGENativeUI
{
    using System;
    using System.IO;
    using System.Drawing;
    using System.Drawing.Imaging;

    public sealed class CustomTextureReference : TextureReference
    {
        internal static readonly TextureDictionary CustomTextureDictionary = "rnui_custom_textures";

        internal uint Id { get; }
        public bool IsUpdatable { get; }

        internal CustomTextureReference(string name, uint id, int width, int height, bool updatable) : base(CustomTextureDictionary, name, width, height)
        {
            Id = id;
            IsUpdatable = updatable;
        }

        public static CustomTextureReference FromFile(string name, string filePath, bool updatable)
        {
            Throw.IfNull(filePath, nameof(filePath));
            if (!File.Exists(filePath)) throw new FileNotFoundException($"The file specified in '{nameof(filePath)}' was not found.", filePath);

            using (Bitmap bitmap = new Bitmap(filePath))
            {
                return FromBitmap(name, bitmap, updatable);
            }
        }

        public static CustomTextureReference FromBitmap(string name, Bitmap bitmap, bool updatable)
        {
            Throw.IfNull(bitmap, nameof(bitmap));
            Throw.InvalidOperationIfNot(bitmap.PixelFormat == PixelFormat.Format32bppArgb, "Invalid pixel format"); // TODO: support other formats

            byte[] dst = new byte[bitmap.Width * 4 * bitmap.Height];
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            
            System.Runtime.InteropServices.Marshal.Copy(bitmapData.Scan0, dst, 0, dst.Length);
            
            bitmap.UnlockBits(bitmapData);

            return FromPixelData(name, bitmap.Width, bitmap.Height, dst, updatable);
        }

        public static unsafe CustomTextureReference FromPixelData(string name, int width, int height, byte[] pixelData, bool updatable)
        {
            Throw.IfNull(name, nameof(name));
            Throw.InvalidOperationIf(RNUI.Helper.DoesCustomTextureExist(name), $"Custom texture with name '{nameof(name)}' already exists.");
            Throw.IfNull(pixelData, nameof(pixelData));
            Throw.ArgumentExceptionIfNot((pixelData.Length == (width * 4 * height)));
            Throw.IfNegativeOrZero(width, nameof(width));
            Throw.IfNegativeOrZero(height, nameof(height));

            uint id;
            fixed (byte* data = pixelData)
            {
                Common.LogDebug($"Calling CreateCustomTexture({name}, {width}, {height}, {((IntPtr)data).ToString("X16")}, {updatable})");
                id = RNUI.Helper.CreateCustomTexture(name, (uint)width, (uint)height, (IntPtr)data, updatable);
                Common.LogDebug($"Id -> {id}");
            }

            CustomTextureReference tex = new CustomTextureReference(name, id, width, height, updatable);
            return tex;
        }
    }
}

