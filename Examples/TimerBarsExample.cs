namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class TimerBarsExample
    {
        [ConsoleCommand(Name = "TimerBarsExample", Description = "Example showing the TimerBars classes.")]
        private static void Command()
        {

            TimerBarsManager timerBarsMgr = new TimerBarsManager();
            timerBarsMgr.TimerBars.Add(new TextTimerBar("TIME", "00:00.00") { IsVisible = true });
            timerBarsMgr.TimerBars.Add(new TextTimerBar("POSICIÓN", "8/8") { IsVisible = true });
            timerBarsMgr.TimerBars.Add(new LabeledTimerBar("LABEL") { IsVisible = true });
            LabeledTimerBar resizableTimerBar = new LabeledTimerBar("RESIZABLE") { IsVisible = true };
            timerBarsMgr.TimerBars.Add(resizableTimerBar);
            timerBarsMgr.TimerBars.Add(new TextTimerBar("LABEL", "TEXT") { IsVisible = true });
            ProgressTimerBar progressBar = new ProgressTimerBar("PROGRESS") { IsVisible = true, Percentage = 0.5f };
            timerBarsMgr.TimerBars.Add(progressBar);

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Add))
                        progressBar.Percentage += 2.0f * Game.FrameTime;
                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Subtract))
                        progressBar.Percentage -= 2.0f * Game.FrameTime;

                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Multiply))
                    {
                        resizableTimerBar.Width += 0.5f * Game.FrameTime;
                        resizableTimerBar.Height += 0.5f * Game.FrameTime;
                    }
                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Divide))
                    {
                        resizableTimerBar.Width -= 0.5f * Game.FrameTime;
                        resizableTimerBar.Height -= 0.5f * Game.FrameTime;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        if (timerBarsMgr.IsAnyTimerBarVisible)
                        {
                            timerBarsMgr.HideAllTimerBars();
                        }
                        else
                        {
                            timerBarsMgr.ShowAllTimerBars();
                        }
                    }
                }
            });
        }
    }
}

