namespace RAGENativeUI.Tests;

using Rage.Native;

using System.Drawing;
using System.Drawing.Imaging;

public class RuntimeTextureDictionaryTests
{
    private const string Txd = "rnui_test_txd";

    [Theory]
    [InlineData(TextureFormat.B8G8R8A8, 4)]
    [InlineData(TextureFormat.R8G8B8A8, 4)]
    //[InlineData(TextureFormat.R8G8, 2)]
    [InlineData(TextureFormat.R8, 1)]
    public void CreateAndModifyTexture(TextureFormat format, int bytesPerPixel)
    {
        using var txd = RegisterTxd();
        txd.AddTexture("my_tex", 8, 8, format, IntPtr.Zero, updatable: true);

        using (var map = txd.MapTexture("my_tex"))
        {
            Assert.Same(txd, map.TextureDictionary);
            Assert.Equal("my_tex", map.TextureName);
            Assert.Equal(format, map.Format);
            Assert.NotEqual(IntPtr.Zero, map.Data);
            Assert.Equal(8, map.Width);
            Assert.Equal(8, map.Height);
            Assert.Equal(bytesPerPixel * 8, map.Stride);
            Assert.Equal(bytesPerPixel * 8, map.BitsPerPixel);

            unsafe
            {
                var d = (byte*)map.Data;
                for (int i = 0; i < map.Stride * map.Height; i += bytesPerPixel)
                {
                    if (bytesPerPixel >= 1) d[i + 0] = 255;
                    if (bytesPerPixel >= 2) d[i + 1] = 0;
                    if (bytesPerPixel >= 3) d[i + 2] = 0;
                    if (bytesPerPixel >= 4) d[i + 3] = 255;
                }
            }
        }

        using (var map = txd.MapTexture("my_tex"))
        {
            unsafe
            {
                var d = (byte*)map.Data;
                for (int i = 0; i < map.Stride * map.Height; i += bytesPerPixel)
                {
                    if (bytesPerPixel >= 1) Assert.Equal(255, d[i + 0]);
                    if (bytesPerPixel >= 2) Assert.Equal(0, d[i + 1]);
                    if (bytesPerPixel >= 3) Assert.Equal(0, d[i + 2]);
                    if (bytesPerPixel >= 4) Assert.Equal(255, d[i + 3]);
                }
            }
        }

        ViewTexture(Txd, "my_tex");
    }

    [Fact]
    public void CreateFromBitmap()
    {
        using var txd = RegisterTxd();
        using var bitmap = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
        {
            using var blackBrush = new SolidBrush(Color.Black);
            using var purpleBrush = new SolidBrush(Color.Purple);
            using var redPen = new Pen(Color.Red, 3.0f);
            using var g = Graphics.FromImage(bitmap);
            g.FillRectangle(blackBrush, 0, 0, 64, 64);
            g.DrawLine(redPen, 0, 0, 64, 64);
            g.DrawLine(redPen, 0, 64, 64, 0);
            g.FillEllipse(purpleBrush, 32 - 8, 32 - 8, 16, 16);
            g.Flush();
        }

        txd.AddTextureFromImage("my_tex", bitmap);

        using (var map = txd.MapTexture("my_tex"))
        {
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Assert.Equal(bmpData.Width, map.Width);
            Assert.Equal(bmpData.Height, map.Height);
            Assert.Equal(bmpData.Stride, map.Stride);

            unsafe
            {

                var e = (byte*)bmpData.Scan0;
                var d = (byte*)map.Data;
                for (int i = 0; i < map.Stride * map.Height; i++)
                {
                    Assert.Equal(e[i], d[i]);
                }
            }

            bitmap.UnlockBits(bmpData);
        }

        ViewTexture(Txd, "my_tex");
    }

    private static RuntimeTextureDictionary RegisterTxd()
    {
        var success = RuntimeTextureDictionary.TryRegister(Txd, out var newTxd);
        Assert.True(success);
        Assert.NotNull(newTxd);
        return newTxd;
    }

    private static void ViewTexture(string txd, string tex, int timeMs = 500)
    {
        GameFiber.ExecuteFor(() =>
        {
            var aspectRatio = NativeFunction.Natives.xF1307EF624A80D87<float>(false);
            var s = 0.2f;
            NativeFunction.Natives.xE7FFAE5EBF23D890(txd, tex, 0.5f, 0.5f, s, s * aspectRatio, 0.0f, 255, 255, 255, 255, false);
        }, timeMs);
        GameFiber.Yield(); // wait for texture to stop drawing before continuing
    }
}
