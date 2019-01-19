namespace Examples
{
#if RPH1
    extern alias rph1;
    using Game = rph1::Rage.Game;
    using Rotator = rph1::Rage.Rotator;
    using Vector2 = rph1::Rage.Vector2;
    using Vector3 = rph1::Rage.Vector3;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Drawing;
    using RAGENativeUI.ImGui;

    internal static class SpriteExample
    {
        [ConsoleCommand(Name = "SpriteExample", Description = "Example showing the Sprite class.")]
        private static void Command()
        {
            RPH.GameFiber.StartNew(() =>
            {
                CustomTexture tex = null;
                for (int i = 0; i < 5; i++)
                {
                    Game.LogTrivial("my_own_texture_0 -> " + CustomTexture.DoesCustomTextureExists("my_own_texture_0"));
                    Game.LogTrivial("my_own_texture_1 -> " + CustomTexture.DoesCustomTextureExists("my_own_texture_1"));
                    Game.LogTrivial("my_own_texture_2 -> " + CustomTexture.DoesCustomTextureExists("my_own_texture_2"));
                    Game.LogTrivial("my_own_texture_3 -> " + CustomTexture.DoesCustomTextureExists("my_own_texture_3"));
                    Game.LogTrivial("my_own_texture_4 -> " + CustomTexture.DoesCustomTextureExists("my_own_texture_4"));
                    Game.LogTrivial("Total -> " + CustomTexture.GetNumberOfCustomTextures());
                    Game.LogTrivial("");

                    Bitmap bmp = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
                    Font f = new Font(FontFamily.GenericSansSerif, 11.0f);
                    Pen pe = new Pen(Color.Aquamarine, 5.0f);
                    Brush b = new SolidBrush(Color.Green);
                    g.Clear(Color.Red);
                    g.DrawEllipse(pe, 50.0f, 50.0f, 40.0f, 25.0f);
                    g.DrawString("Custom texture from Bitmap! -> " + i, f, b, new PointF(5.0f, 5.0f));

                    f.Dispose();
                    pe.Dispose();
                    b.Dispose();
                    g.Dispose();
                    tex = CustomTexture.FromBitmap("my_own_texture_" + i, bmp, false);

                    bmp.Dispose();

                    RPH.GameFiber.Yield();
                }
                Game.LogTrivial("Finished texture creation");

                foreach (TextureReference t in CustomTexture.CustomTexturesDictionary.GetTextures())
                {
                    Game.LogTrivial($"Is CustomTexture -> {t is CustomTexture}");
                    Game.LogTrivial($"Name -> {t.Name}");
                    Game.LogTrivial($"Dictionary -> {t.Dictionary.Name}");
                    Game.LogTrivial($"Width -> {t.Width}");
                    Game.LogTrivial($"Height -> {t.Height}");
                    if (t is CustomTexture t2)
                    {
                        Game.LogTrivial($"IsUpdatable -> {t2.IsUpdatable}");
                        Game.LogTrivial($"IsValid -> {t2.IsValid}");
                    }
                    Game.LogTrivial("");
                }

                Sprite sprite = new Sprite(tex.Dictionary, tex.Name, (0.5f, 0.5f).Rel(), (128.0f, 128.0f).Px());
                Game.LogTrivial($"TextureDictionary: {sprite.TextureDictionary.Name}");
                Game.LogTrivial($"TextureName: {sprite.TextureName}");
                Game.LogTrivial($"Position: {sprite.Position}");
                Game.LogTrivial($"Size: {sprite.Size}");
                Game.LogTrivial($"Rotation: {sprite.Rotation}");
                Game.LogTrivial($"Color: {sprite.Color}");
                sprite.IsVisible = true;

                Sprite3D s = new Sprite3D(tex.Dictionary, tex.Name);
                s.BackFace = true;
                s.UV = new UVCoords(0.25f, 0.25f, 0.75f, 0.75f);

                Rect3D r = new Rect3D();
                r.BackFace = true;

                float pitch = 0.0f, roll = 0.0f, yaw = 0.0f, scaleX = 1.0f, scaleY = 1.0f, zoom = 0.0f;

                while (true)
                {
                    RPH.GameFiber.Yield();

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        sprite.Rotation += 50f * Game.FrameTime;
                    }
                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        sprite.Rotation -= 50f * Game.FrameTime;
                    }
                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        sprite.Color = s.Color = Color.FromArgb(RPH.MathHelper.GetRandomInteger(0, 255), RPH.MathHelper.GetRandomInteger(0, 255), RPH.MathHelper.GetRandomInteger(0, 255), RPH.MathHelper.GetRandomInteger(0, 255));
                        r.Color = Color.FromArgb(RPH.MathHelper.GetRandomInteger(0, 255), RPH.MathHelper.GetRandomInteger(0, 255), RPH.MathHelper.GetRandomInteger(0, 255), RPH.MathHelper.GetRandomInteger(0, 255));
                    }

                    sprite.Draw();

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad1))
                        zoom += 0.5f * Game.FrameTime;
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad3))
                        zoom -= 0.5f * Game.FrameTime;

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad8))
                        pitch += 30.0f * Game.FrameTime;
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad2))
                        pitch -= 30.0f * Game.FrameTime;

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad7))
                        roll += 30.0f * Game.FrameTime;
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad9))
                        roll -= 30.0f * Game.FrameTime;

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad4))
                        yaw += 30.0f * Game.FrameTime;
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.NumPad6))
                        yaw -= 30.0f * Game.FrameTime;

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                        scaleX += 5.0f * Game.FrameTime;
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                        scaleX -= 5.0f * Game.FrameTime;

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Multiply))
                        scaleY += 5.0f * Game.FrameTime;
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Divide))
                        scaleY -= 5.0f * Game.FrameTime;

                    s.UV = new UVCoords(zoom, zoom, 1.0f - zoom, 1.0f - zoom);

                    Vector3 p = Game.LocalPlayer.Character.GetOffsetPositionFront(2.0f);
                    s.SetTransform(p, new Vector2(scaleX, scaleY), new Rotator(pitch, roll, yaw).ToQuaternion());

                    p.Z = s.BottomLeft.Z;
                    r.SetTransform(p, new Vector2(scaleX, scaleY), new Rotator(pitch + 90.0f, roll, yaw).ToQuaternion());

                    r.Draw();
                    s.Draw();
                }
            });
        }
    }
}

