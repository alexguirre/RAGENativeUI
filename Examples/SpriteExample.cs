namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using RAGENativeUI.ImGui;

    internal static class SpriteExample
    {
        struct KeyPos
        {
            public Vector3 pos;
            public Vector3 up;
            public Vector3 forward;
            public float roll;
        }

        [ConsoleCommand(Name = "SpriteExample", Description = "Example showing the Sprite class.")]
        private static void Command()
        {
            Sprite sprite = new Sprite("3dtextures", "mpgroundlogo_cops", ScreenRectangle.FromAbsoluteCoords(1920f / 2f - 64f, 1080f / 2f - 64f, 128f, 128f));
            Game.LogTrivial($"TextureDictionary: {sprite.TextureDictionary.Name}");
            Game.LogTrivial($"TextureName: {sprite.TextureName}");
            Game.LogTrivial($"Rectangle: {sprite.Rectangle}");
            Game.LogTrivial($"Rotation: {sprite.Rotation}");
            Game.LogTrivial($"Color: {sprite.Color}");
            sprite.IsVisible = true;

            Sprite3D s = new Sprite3D("3dtextures", "mpgroundlogo_cops");
            s.BackFace = true;
            s.UV = new UVCoords(0.25f, 0.25f, 0.75f, 0.75f);

            Rect3D r = new Rect3D();
            r.BackFace = true;

            GameFiber.StartNew(() =>
            {
                KeyPos[] positions = new KeyPos[125];

                float pitch = 0.0f, roll = 0.0f, yaw = 0.0f, scaleX = 1.0f, scaleY = 1.0f, zoom = 0.0f;

                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        sprite.Rotation += 50f * Game.FrameTime;
                    }
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        sprite.Rotation -= 50f * Game.FrameTime;
                    }
                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        sprite.Color = s.Color = Color.FromArgb(MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255));
                        r.Color = Color.FromArgb(MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255));
                    }

                    sprite.Draw();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad1))
                        zoom += 0.5f * Game.FrameTime;
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad3))
                        zoom -= 0.5f * Game.FrameTime;

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad8))
                        pitch += 30.0f * Game.FrameTime;
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad2))
                        pitch -= 30.0f * Game.FrameTime;

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad7))
                        roll += 30.0f * Game.FrameTime;
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad9))
                        roll -= 30.0f * Game.FrameTime;

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad4))
                        yaw += 30.0f * Game.FrameTime;
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.NumPad6))
                        yaw -= 30.0f * Game.FrameTime;

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                        scaleX += 5.0f * Game.FrameTime;
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                        scaleX -= 5.0f * Game.FrameTime;

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Multiply))
                        scaleY += 5.0f * Game.FrameTime;
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Divide))
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

