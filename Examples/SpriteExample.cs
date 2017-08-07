namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using RAGENativeUI.Utility;

    internal static class SpriteExample
    {
        [ConsoleCommand(Name = "SpriteExample", Description = "Example showing the Sprite class.")]
        private static void Command()
        {
            GameScreenRectangle fromAbsolute = GameScreenRectangle.FromAbsoluteCoords(1920f / 4f, 1080f / 4f, 1920f / 2f, 1080f / 2f);
            GameScreenRectangle fromRelative = GameScreenRectangle.FromRelativeCoords(0.5f, 0.5f, 0.5f, 0.5f);

            TextureDictionary dict = new TextureDictionary("gtav_online");
            Game.LogTrivial(dict.Name);
            foreach (string name in dict.TextureNames)
            {
                Game.LogTrivial("   " + name);
            }

            Sprite sprite = new Sprite("3dtextures", "mpgroundlogo_cops", GameScreenRectangle.FromAbsoluteCoords(1920f / 2f - 64f, 1080f / 2f - 64f, 128f, 128f));
            Game.LogTrivial($"TextureDictionary: {sprite.TextureDictionary.Name}");
            Game.LogTrivial($"TextureName: {sprite.TextureName}");
            Game.LogTrivial($"Rectangle: {sprite.Rectangle}");
            Game.LogTrivial($"Resolution: {sprite.Resolution}");
            Game.LogTrivial($"Rotation: {sprite.Rotation}");
            Game.LogTrivial($"Color: {sprite.Color}");
            sprite.IsVisible = true;

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                    
                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Add))
                    {
                        sprite.Rotation += 50f * Game.FrameTime;
                    }
                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Subtract))
                    {
                        sprite.Rotation -= 50f * Game.FrameTime;
                    }
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        sprite.Color = Color.FromArgb(MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255), MathHelper.GetRandomInteger(0, 255));
                    }

                    sprite.Draw();
                }
            });
        }
    }
}

