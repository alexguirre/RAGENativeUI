namespace RNUIExamples
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;

    using Rage;
    using Rage.Native;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class Util
    {
        /// <summary>
        /// Create new list menu item containing all the <see cref="HudColor"/>s which can be selected by the user.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static UIMenuListScrollerItem<HudColor> NewColorsItem(string text, string description)
            => new UIMenuListScrollerItem<HudColor>(text, description, (HudColor[])Enum.GetValues(typeof(HudColor)))
            {
                // custom formatter that adds whitespace between words (i.e. "RedDark" -> "Red Dark")
                Formatter = v => v.ToString().Aggregate("", (acc, c) => acc + (acc.Length > 0 && char.IsUpper(c) ? " " : "") + c)
            };

        public static UIMenuCheckboxItem NewTriStateCheckbox(string name, string description)
        {
            var cb = new UIMenuCheckboxItem(name, false, description);

            cb.Activated += (s, i) =>
            {
                if (cb.Checked)
                {
                    if (cb.Style == UIMenuCheckboxStyle.Tick)
                    {
                        cb.Style = UIMenuCheckboxStyle.Cross;
                        cb.Checked = !cb.Checked; // make the item checked
                    }
                    else
                    {
                        cb.Style = UIMenuCheckboxStyle.Tick;
                    }
                }
            };

            return cb;
        }

        [Rage.Attributes.ConsoleCommand]
        public static void TestTxd()
        {
            using var txd = new GameTextureDictionary("my_rnui_txd");
            txd.AddTextureFromDDS("my_tex1", "Plugins\\some_image.dds");
            txd.AddTextureFromDDS("my_tex2", "Plugins\\some_image.dds");
            txd.AddTextureFromDDS("my_tex3", "Plugins\\some_image.dds");
            txd.AddTextureFromDDS("my_tex4", new byte[] { 1, 2, 3, 4 }); // invalid DDS
            {
                using var bitmap = new Bitmap(64, 64, PixelFormat.Format32bppArgb);
                {
                    using var blackBrush = new SolidBrush(Color.Black);
                    using var purpleBrush = new SolidBrush(Color.Purple);
                    using var redPen = new Pen(Color.Red, 3.0f);
                    using var g = System.Drawing.Graphics.FromImage(bitmap);
                    g.FillRectangle(blackBrush, 0, 0, 64, 64);
                    g.DrawLine(redPen, 0, 0, 64, 64);
                    g.DrawLine(redPen, 0, 64, 64, 0);
                    g.FillEllipse(purpleBrush, 32 - 8, 32 - 8, 16, 16);
                    g.Flush();
                }

                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0,0,64,64), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                txd.AddTexture("my_tex5", (uint)bitmapData.Width, (uint)bitmapData.Height, GameTextureFormat.B8G8R8A8, bitmapData.Scan0);
                txd.AddTexture("my_tex6", (uint)bitmapData.Width, (uint)bitmapData.Height, GameTextureFormat.R8G8B8A8, bitmapData.Scan0);

                bitmap.UnlockBits(bitmapData);
            }

            foreach (var texture in txd.Textures)
            {
                Game.LogTrivial(" > " + texture);
            }

            if (txd.MapTexture("my_tex6", out var map))
            {
                Game.LogTrivial(" Mapped texture");
                Game.LogTrivial($"  > Data          = {map.Data.ToString("X")}");
                Game.LogTrivial($"  > Width         = {map.Width}");
                Game.LogTrivial($"  > Height        = {map.Height}");
                Game.LogTrivial($"  > Stride        = {map.Stride}");
                Game.LogTrivial($"  > BitsPerPixel  = {map.BitsPerPixel}");
                Game.LogTrivial($"  > Stride*Height = {map.Stride*map.Height}");

                if (map.Data != IntPtr.Zero)
                {
                    unsafe
                    {
                        Game.LogTrivial($"  {(((uint*)map.Data)[0]):X8}");
                        Game.LogTrivial($"  {(*(uint*)&((byte*)map.Data)[map.Stride * map.Height / 2 + map.Stride / 2]):X8}");
                        for (int i = 0; i < (map.Stride * map.Height); i++)
                            ((byte*)map.Data)[i] = 0;
                    }
                }

                txd.UnmapTexture("my_tex6", map);
            }
            else
            {
                Game.LogTrivial(" Failed to map texture");
            }

            var r = 0.0f;
            while (true)
            {
                GameFiber.Yield();
                var aspectRatio = NativeFunction.Natives.xF1307EF624A80D87<float>(false);
                var s = 0.1f;
                r = (r + 20.0f * Game.FrameTime) % 360.0f;
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex1", 0.25f, 0.2f, s, s * aspectRatio, r, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex2", 0.25f, 0.4f, s, s * aspectRatio, r, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex3", 0.25f, 0.6f, s, s * aspectRatio, r, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex4", 0.25f, 0.8f, s, s * aspectRatio, r, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex5", 0.75f, 0.4f, s, s * aspectRatio, r, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex6", 0.75f, 0.6f, s, s * aspectRatio, r, 255, 255, 255, 255, false);
            }
        }
    }
}
