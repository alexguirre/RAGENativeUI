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
        public static unsafe void TestTxd()
        {
            using var txd = new GameTextureDictionary("my_rnui_txd");
            txd.AddTextureFromDDS("my_tex1", "Plugins\\some_image.dds");
            txd.AddTextureFromDDS("my_tex2", "Plugins\\some_image.dds");
            txd.AddTextureFromDDS("my_tex3", "Plugins\\some_image_rgba.dds", updatable: true);
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

                var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, 64, 64), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                txd.AddTexture("my_tex5", (uint)bitmapData.Width, (uint)bitmapData.Height, GameTextureFormat.B8G8R8A8, bitmapData.Scan0);
                txd.AddTexture("my_tex6", (uint)bitmapData.Width, (uint)bitmapData.Height, GameTextureFormat.B8G8R8A8, bitmapData.Scan0, updatable: true);

                bitmap.UnlockBits(bitmapData);
            }
            txd.AddTexture("my_tex7", 64, 64, GameTextureFormat.R8G8, IntPtr.Zero, updatable: true);

            foreach (var texture in txd.Textures)
            {
                Game.LogTrivial(" > " + texture);
            }

            if (txd.MapTexture("my_tex6", out var map))
            {
                Game.LogTrivial(" Mapped texture");
                Game.LogTrivial($"  > Data          = {map.Data.ToString("X")}");
                Game.LogTrivial($"  > Format        = {map.DXGIFormat}");
                Game.LogTrivial($"  > Width         = {map.Width}");
                Game.LogTrivial($"  > Height        = {map.Height}");
                Game.LogTrivial($"  > Stride        = {map.Stride}");
                Game.LogTrivial($"  > BitsPerPixel  = {map.BitsPerPixel}");
                Game.LogTrivial($"  > Stride*Height = {map.Stride * map.Height}");

                var rnd = new Random();
                var numPixels = map.Stride * map.Height / (map.BitsPerPixel / 8);
                for (int i = 0; i < numPixels; i++)
                {
                    var x = i % map.Width;
                    var y = i / map.Width;

                    var pixel = (byte*)&((int*)map.Data)[i];
                    pixel[0] = (byte)(rnd.Next() % 255);
                    pixel[1] = (byte)(rnd.Next() % 255);
                    pixel[2] = (byte)(rnd.Next() % 255);
                    //((uint*)map.Data)[i] = 0xFFFFFFFF ;
                }

                txd.UnmapTexture("my_tex6", map);
            }

            if (txd.MapTexture("my_tex3", out map))
            {
                Game.LogTrivial(" Mapped texture");
                Game.LogTrivial($"  > Data          = {map.Data.ToString("X")}");
                Game.LogTrivial($"  > Format        = {map.DXGIFormat}");
                Game.LogTrivial($"  > Width         = {map.Width}");
                Game.LogTrivial($"  > Height        = {map.Height}");
                Game.LogTrivial($"  > Stride        = {map.Stride}");
                Game.LogTrivial($"  > BitsPerPixel  = {map.BitsPerPixel}");
                Game.LogTrivial($"  > Stride*Height = {map.Stride * map.Height}");

                var rnd = new Random();
                var numPixels = map.Stride * map.Height / (map.BitsPerPixel / 8);
                for (int i = 0; i < numPixels; i++)
                {
                    var x = i % map.Width;
                    var y = i / map.Width;

                    var pixel = (byte*)&((int*)map.Data)[i];
                    pixel[0] = (byte)(pixel[0] + 50);
                    pixel[1] = (byte)(pixel[1] + 50);
                    pixel[2] = (byte)(pixel[2] + 50);
                    //((uint*)map.Data)[i] = 0xFFFFFFFF ;
                }

                txd.UnmapTexture("my_tex3", map);
            }


            var rot = 0.0f;
            var hue = 0.0f;
            while (true)
            {
                GameFiber.Yield();

                hue = (hue + 60.0f * Game.FrameTime) % 360.0f;
                if (txd.MapTexture("my_tex6", out map))
                {
                    unsafe
                    {
                        var numPixels = map.Stride * map.Height / 4;
                        for (int i = 0; i < numPixels; i++)
                        {
                            var x = i % map.Width;
                            var y = i / map.Width;

                            var saturation = 1.0f - (float)y / map.Width;
                            var value = (float)x / map.Height;
                            var c = ColorFromHSV(hue, saturation, value);
                            ((int*)map.Data)[i] = c.ToArgb();
                        }
                    }

                    txd.UnmapTexture("my_tex6", map);
                }


                var aspectRatio = NativeFunction.Natives.xF1307EF624A80D87<float>(false);
                var s = 0.1f;
                //r = (r + 20.0f * Game.FrameTime) % 360.0f;
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex1", 0.25f, 0.2f, s, s * aspectRatio, rot, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex2", 0.25f, 0.4f, s, s * aspectRatio, rot, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex3", 0.25f, 0.6f, s, s * aspectRatio, rot, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex4", 0.25f, 0.8f, s, s * aspectRatio, rot, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex5", 0.75f, 0.4f, s, s * aspectRatio, rot, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex6", 0.75f, 0.6f, s, s * aspectRatio, rot, 255, 255, 255, 255, false);
                NativeFunction.Natives.xE7FFAE5EBF23D890("my_rnui_txd", "my_tex7", 0.75f, 0.8f, s, s * aspectRatio, rot, 255, 255, 255, 255, false);
            }
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            else if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            else if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            else if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            else if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            else
                return Color.FromArgb(255, v, p, q);
        }
    }
}
