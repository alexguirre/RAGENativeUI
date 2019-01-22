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
            //TextTimerBar timeBar = new TextTimerBar("TIEMPO", "00:00.00");
            TextTimerBar positionBar = new TextTimerBar("POSICIÓN", "8/8");
            LabeledTimerBar labelBar = new LabeledTimerBar("LABEL") { LabelColor = HudColor.Red.GetColor() };
            TextTimerBar textBar = new TextTimerBar("LABEL", "TEXT") { TextColor = HudColor.Red.GetColor(), HighlightColor = HudColor.Red.GetColor() };
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
            new LabeledTimerBar(nameof(TimerBarIcons.RP)) { Icon = TimerBarIcons.RP };
            new LabeledTimerBar(nameof(TimerBarIcons.Rockets)) { Icon = TimerBarIcons.Rockets };
            new LabeledTimerBar(nameof(TimerBarIcons.Spikes)) { Icon = TimerBarIcons.Spikes };
            new LabeledTimerBar(nameof(TimerBarIcons.Boost)) { Icon = TimerBarIcons.Boost };
            new LabeledTimerBar(nameof(TimerBarIcons.HomingRocket)) { Icon = TimerBarIcons.HomingRocket };
            new LabeledTimerBar(nameof(TimerBarIcons.Hop)) { Icon = TimerBarIcons.Hop };
            new LabeledTimerBar(nameof(TimerBarIcons.MachineGun)) { Icon = TimerBarIcons.MachineGun };
            new LabeledTimerBar(nameof(TimerBarIcons.Parachute)) { Icon = TimerBarIcons.Parachute };
            new LabeledTimerBar(nameof(TimerBarIcons.RocketBoost)) { Icon = TimerBarIcons.RocketBoost };
            new LabeledTimerBar(nameof(TimerBarIcons.Tick)) { Icon = TimerBarIcons.Tick };
            new LabeledTimerBar(nameof(TimerBarIcons.Cross)) { Icon = TimerBarIcons.Cross };
            new LabeledTimerBar(nameof(TimerBarIcons.Beast)) { Icon = TimerBarIcons.Beast };
            new LabeledTimerBar(nameof(TimerBarIcons.Random)) { Icon = TimerBarIcons.Random };
            new LabeledTimerBar(nameof(TimerBarIcons.SlowTime)) { Icon = TimerBarIcons.SlowTime };
            new LabeledTimerBar(nameof(TimerBarIcons.Swap)) { Icon = TimerBarIcons.Swap };
            new LabeledTimerBar(nameof(TimerBarIcons.Testosterone)) { Icon = TimerBarIcons.Testosterone };
            new LabeledTimerBar(nameof(TimerBarIcons.Thermal)) { Icon = TimerBarIcons.Thermal };
            new LabeledTimerBar(nameof(TimerBarIcons.Weed)) { Icon = TimerBarIcons.Weed };
            new LabeledTimerBar(nameof(TimerBarIcons.Hidden)) { Icon = TimerBarIcons.Hidden };
            new LabeledTimerBar(nameof(TimerBarIcons.Time)) { Icon = TimerBarIcons.Time };


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

