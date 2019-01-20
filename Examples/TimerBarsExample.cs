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
    using RAGENativeUI.TimerBars;

    internal static class TimerBarsExample
    {
        [ConsoleCommand(Name = "TimerBarsExample", Description = "Example showing the TimerBars classes.")]
        private static void Command()
        {
            TextTimerBar timeBar = new TextTimerBar("TIEMPO", "00:00.00");
            TextTimerBar positionBar = new TextTimerBar("POSICIÓN", "8/8");
            LabeledTimerBar labelBar = new LabeledTimerBar("LABEL") { LabelColor = HudColor.Red.GetColor() };
            TextTimerBar textBar = new TextTimerBar("LABEL", "TEXT") { TextColor = HudColor.Red.GetColor(), HighlightColor = HudColor.Red.GetColor() };
            ProgressTimerBar progressBar = new ProgressTimerBar("") { Percentage = 0.5f };
            progressBar.Markers.Add(0.3333f);
            progressBar.Markers.Add(0.6666f);

            RPH.GameFiber.StartNew(() =>
            {
                while (true)
                {
                    RPH.GameFiber.Yield();

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                        labelBar.IsVisible = !labelBar.IsVisible;

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                        progressBar.Percentage += 2.0f * RPH.Game.FrameTime;
                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                        progressBar.Percentage -= 2.0f * RPH.Game.FrameTime;

                    progressBar.Label = $"{progressBar.Percentage:P}";
                }
            });
        }
    }
}

