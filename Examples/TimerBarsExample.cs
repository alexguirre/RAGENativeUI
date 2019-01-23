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
            TextTimerBar textBar = new TextTimerBar("LABEL", "TEXT") { TextColor = HudColor.Red.GetColor(), HighlightColor = HudColor.Red.GetColor(), Icon = TimerBarIcons.Hop };
            ProgressTimerBar progressBar = new ProgressTimerBar("") { Percentage = 0.5f };
            progressBar.Markers.Add(0.3333f);
            progressBar.Markers.Add(0.6666f);
            CheckpointsTimerBar checkpointsBar = new CheckpointsTimerBar("CHECKPOINTS", 8) { LabelColor = HudColor.Blue.GetColor() };
            for (int i = 0; i < checkpointsBar.Checkpoints.Length; i++)
            {
                checkpointsBar.Checkpoints[i].IsCompleted = false;
                checkpointsBar.Checkpoints[i].CompletedColor = HudColor.Blue.GetColor();
                checkpointsBar.Checkpoints[i].HasCross = false;
                checkpointsBar.Checkpoints[i].CrossColor = HudColor.Black.GetColor();
            }
            checkpointsBar.Checkpoints[1].IsCompleted = true;
            checkpointsBar.Checkpoints[2].HasCross = true;
            checkpointsBar.Checkpoints[3].IsCompleted = true;
            checkpointsBar.Checkpoints[3].HasCross = true;
            checkpointsBar.Checkpoints[6].IsCompleted = true;
            IconsTimerBar iconsBar = new IconsTimerBar("ICONS");
            iconsBar.Icons.Add(TimerBarIcons.Rockets);
            iconsBar.Icons.Add(TimerBarIcons.Rockets);
            iconsBar.Icons.Add(TimerBarIcons.Rockets);
            iconsBar.Icons.Add(TimerBarIcons.Rockets);
            iconsBar.Icons.Add(TimerBarIcons.Rockets);
            Color c = iconsBar.Icons[2].Color;
            c = Color.FromArgb(51, c.R, c.G, c.B);
            iconsBar.Icons[2].Color = c;
            iconsBar.Icons[3].Color = c;
            iconsBar.Icons[4].Color = c;


            labelBar.OrderPriority = 0;
            progressBar.OrderPriority = 1;

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

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.U))
                    {
                        labelBar.OrderPriority = 2;
                        textBar.OrderPriority = 0;
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.I))
                    {
                        if (positionBar != null)
                        {
                            positionBar.Dispose();
                            positionBar = null;
                        }
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.O))
                    {
                        checkpointsBar.VisibleCheckpoints = checkpointsBar.VisibleCheckpoints == 8 ? 4 : 8;
                    }
                }
            });
        }
    }
}

