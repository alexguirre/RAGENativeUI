namespace Examples
{
#if RPH1
    extern alias rph1;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

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

            RPH.GameFiber.StartNew(() =>
            {
                while (true)
                {
                    RPH.GameFiber.Yield();

                    stack.Draw();
                    middleBar.Draw();

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                        progressBar.Percentage += 2.0f * RPH.Game.FrameTime;
                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                        progressBar.Percentage -= 2.0f * RPH.Game.FrameTime;

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Multiply))
                    {
                        textBar.Rectangle = ScreenRectangle.FromRelativeCoords(textBar.Rectangle.X, textBar.Rectangle.Y, textBar.Rectangle.Width + 0.5f * RPH.Game.FrameTime, textBar.Rectangle.Height + 0.5f * RPH.Game.FrameTime);
                    }
                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Divide))
                    {
                        textBar.Rectangle = ScreenRectangle.FromRelativeCoords(textBar.Rectangle.X, textBar.Rectangle.Y, textBar.Rectangle.Width - 0.5f * RPH.Game.FrameTime, textBar.Rectangle.Height - 0.5f * RPH.Game.FrameTime);
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
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

