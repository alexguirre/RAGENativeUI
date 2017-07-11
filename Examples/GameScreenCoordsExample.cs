namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Native;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Menus;
    using RAGENativeUI.Menus.Rendering;
    using RAGENativeUI.Utility;

    internal static class GameScreenCoordsExample
    {
        [ConsoleCommand(Name = "GameScreenCoordsExample", Description = "Example showing GameScreenCoords struct.")]
        private static void Command()
        {
            GameScreenCoords fromAbsolute = GameScreenCoords.FromAbsoluteCoords(1920f / 4f, 1080f / 4f, 1920f / 2f, 1080f / 2f);
            GameScreenCoords fromRelative = GameScreenCoords.FromRelativeCoords(0.5f, 0.5f, 0.5f, 0.5f);

            Game.LogTrivial("from absolute");
            Game.LogTrivial("   Rel: " + fromAbsolute.Relative);
            Game.LogTrivial("from relative");
            Game.LogTrivial("   Rel: " + fromRelative.Relative);

            float w = 0.5f, h = 0.5f;

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    fromRelative = GameScreenCoords.FromRelativeCoords(0.5f, 0.5f, w, h);

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

                    RectangleF r = fromAbsolute.Relative;
                    NativeFunction.Natives.DrawRect(r.X, r.Y, r.Width, r.Height, 255, 0, 0, 100);

                    r = fromRelative.Relative;
                    NativeFunction.Natives.DrawRect(r.X, r.Y, r.Width, r.Height, 0, 255, 0, 100);
                }
            });
        }
    }
}

