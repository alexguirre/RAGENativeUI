namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Elements.TimerBars;

    internal static class TimerBarsExample
    {
        [ConsoleCommand(Name = "TimerBarsExample", Description = "Example showing the TimerBars classes.")]
        private static void Command()
        {
            TimerBarsStack stack = new TimerBarsStack();

            TextTimerBar timeBar = new TextTimerBar("TIME", "00:00.00");
            TextTimerBar positionBar = new TextTimerBar("POSITION", "8/8") { Color = Color.Cyan };
            LabeledTimerBar labelBar = new LabeledTimerBar("LABEL");
            TextTimerBar textBar = new TextTimerBar("LABEL", "TEXT") { Color = Color.Orange };
            ProgressTimerBar progressBar = new ProgressTimerBar("PROGRESS") { Percentage = 0.5f, Color = Color.Aquamarine };

            stack.Add(timeBar);
            stack.Add(positionBar);
            stack.Add(labelBar);
            stack.Add(textBar);
            stack.Add(progressBar);


            LabeledTimerBar middleBar = new LabeledTimerBar("MIDDLE");
            middleBar.Rectangle = ScreenRectangle.FromRelativeCoords(0.5f, 0.5f, middleBar.Rectangle.Width, middleBar.Rectangle.Height);

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    stack.Draw();
                    middleBar.Draw();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                        progressBar.Percentage += 2.0f * Game.FrameTime;
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                        progressBar.Percentage -= 2.0f * Game.FrameTime;

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Multiply))
                    {
                        textBar.Rectangle = ScreenRectangle.FromRelativeCoords(textBar.Rectangle.X, textBar.Rectangle.Y, textBar.Rectangle.Width + 0.5f * Game.FrameTime, textBar.Rectangle.Height + 0.5f * Game.FrameTime);
                    }
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Divide))
                    {
                        textBar.Rectangle = ScreenRectangle.FromRelativeCoords(textBar.Rectangle.X, textBar.Rectangle.Y, textBar.Rectangle.Width - 0.5f * Game.FrameTime, textBar.Rectangle.Height - 0.5f * Game.FrameTime);
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        if (stack.OriginPosition.HasValue)
                        {
                            stack.OriginPosition = null;
                        }
                        else
                        {
                            stack.OriginPosition = ScreenPosition.FromRelativeCoords(0.5f, 0.5f);
                        }
                    }
                }
            });
        }
    }
}

