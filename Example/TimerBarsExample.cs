[assembly: Rage.Attributes.Plugin("TimerBarsExample", Author = "alexguirre", Description = "Example using RAGENativeUI")]

namespace TimerBarsExampleProject
{
    using Rage;
    using RAGENativeUI.Elements;
    using System.Windows.Forms;
    using System.Drawing;

    public static class EntryPoint
    {
        private static TimerBarPool timerBarPool;

        private static BarTimerBar barTimerBar;
        private static TextTimerBar textTimerBar;
        private static TextTimerBar textTimerBar2;

        public static void Main()
        {
            Game.FrameRender += Process;

            timerBarPool = new TimerBarPool();

            barTimerBar = new BarTimerBar("BAR");
            textTimerBar = new TextTimerBar("TIME", "00:00");
            textTimerBar2 = new TextTimerBar("SPEED", "0 km/h");

            timerBarPool.Add(barTimerBar);
            timerBarPool.Add(textTimerBar);
            timerBarPool.Add(textTimerBar2);

            while (true)
                GameFiber.Yield();
        }


        public static void Process(object sender, GraphicsEventArgs e)
        {
            timerBarPool.Draw();

            if (Game.IsKeyDown(Keys.F5))
            {
                GameFiber.StartNew(delegate
                {
                    for (float i = 0f; i < 1f; i += 0.001f)
                    {
                        barTimerBar.Percentage = i;
                        GameFiber.Sleep(10);
                    }
                });
            }

            if (Game.IsKeyDown(Keys.F6))
            {
                barTimerBar.ForegroundColor = Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                barTimerBar.BackgroundColor = ControlPaint.Dark(barTimerBar.ForegroundColor);
            }

            textTimerBar.Text = World.TimeOfDay.ToString("c");

            textTimerBar2.Text = MathHelper.ConvertMetersPerSecondToKilometersPerHourRounded(Game.LocalPlayer.Character.Speed) + " km/h";

        }

    }
}

