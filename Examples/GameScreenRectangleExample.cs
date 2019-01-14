namespace Examples
{
#if RPH1
    extern alias rph1;
    using GameFiber = rph1::Rage.GameFiber;
    using Game = rph1::Rage.Game;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

    using System.Drawing;

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

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        w += 0.5f * Game.FrameTime;
                        h += 0.5f * Game.FrameTime;
                    }
                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
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

