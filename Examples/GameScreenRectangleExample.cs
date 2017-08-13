namespace Examples
{
    using Rage;
    using Rage.Native;
    using Rage.Attributes;

    using RAGENativeUI;

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

                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Add))
                    {
                        w += 0.5f * Game.FrameTime;
                        h += 0.5f * Game.FrameTime;
                    }
                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Subtract))
                    {
                        w -= 0.5f * Game.FrameTime;
                        h -= 0.5f * Game.FrameTime;
                    }
                    
                    NativeFunction.Natives.DrawRect(fromAbsolute.X, fromAbsolute.Y, fromAbsolute.Width, fromAbsolute.Height, 255, 0, 0, 100);
                    
                    NativeFunction.Natives.DrawRect(fromRelative.X, fromRelative.Y, fromRelative.Width, fromRelative.Height, 0, 255, 0, 100);
                }
            });
        }
    }
}

