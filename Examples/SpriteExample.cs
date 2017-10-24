namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class SpriteExample
    {
        [ConsoleCommand(Name = "SpriteExample", Description = "Example showing the Sprite class.")]
        private static void Command()
        {
            Sprite sprite = new Sprite("3dtextures", "mpgroundlogo_cops", ScreenRectangle.FromAbsoluteCoords(1920f / 2f - 64f, 1080f / 2f - 64f, 128f, 128f));
            Game.LogTrivial($"TextureDictionary: {sprite.Texture.Name}");
            Game.LogTrivial($"TextureName: {sprite.Texture.Dictionary.Name}");
            Game.LogTrivial($"Rectangle: {sprite.Rectangle}");
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

