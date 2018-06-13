namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Native;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class GameScreenRectangleExample
    {
        [ConsoleCommand(Name = "GameScreenRectangleExample", Description = "Example showing GameScreenCoords struct.")]
        private static void Command()
        {
            ScreenRectangle fromAbsolute = ScreenRectangle.FromAbsoluteCoords(1920f / 4f, 1080f / 4f, 1920f / 2f, 1080f / 2f);
            ScreenRectangle fromRelative = ScreenRectangle.FromRelativeCoords(0.5f, 0.5f, 0.5f, 0.5f);

            Game.LogTrivial("from absolute: " + fromAbsolute);
            Game.LogTrivial("from relative: " + fromRelative);

            float w = 0.5f, h = 0.5f;

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    fromRelative = ScreenRectangle.FromRelativeCoords(0.5f, 0.5f, w, h);

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        w += 0.5f * Game.FrameTime;
                        h += 0.5f * Game.FrameTime;
                    }
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        w -= 0.5f * Game.FrameTime;
                        h -= 0.5f * Game.FrameTime;
                    }
                    
                    Rect.Draw(fromAbsolute, Color.FromArgb(255, 0, 0, 100));
                    
                    Rect.Draw(fromRelative, Color.FromArgb(0, 255, 0, 100));
                }
            });
        }
    }
}

