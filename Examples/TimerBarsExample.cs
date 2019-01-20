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
            TimerBarsStack stack = new TimerBarsStack();

            TextTimerBar timeBar = new TextTimerBar("TIEMPO", "00:00.00");
            TextTimerBar positionBar = new TextTimerBar("POSICIÓN", "8/8");
            LabeledTimerBar labelBar = new LabeledTimerBar("LABEL");
            TextTimerBar textBar = new TextTimerBar("LABEL", "TEXT") { HighlightColor = Color.Red };
            ProgressTimerBar progressBar = new ProgressTimerBar("PROGRESS") { Percentage = 0.5f }; 

            stack.Add(timeBar);
            stack.Add(positionBar);
            stack.Add(labelBar);
            stack.Add(textBar);
            stack.Add(progressBar);

            RPH.GameFiber.StartNew(() =>
            {
                while (true)
                {
                    RPH.GameFiber.Yield();

                    stack.Draw();

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                        progressBar.Percentage += 2.0f * RPH.Game.FrameTime;
                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                        progressBar.Percentage -= 2.0f * RPH.Game.FrameTime;
                }
            });
        }
    }
}

