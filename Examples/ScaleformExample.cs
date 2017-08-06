namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Native;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using RAGENativeUI.Utility;

    internal static class ScaleformExample
    {
        [ConsoleCommand(Name = "ScaleformExample", Description = "Example showing the Scaleform class.")]
        private static void Command()
        {
            GameScreenRectangle rect = GameScreenRectangle.FromRelativeCoords(0.5f, 0.5f, 0.5f, 0.5f);
            Scaleform sc = new Scaleform("mp_car_stats_01");
            while (!sc.IsLoaded)
            {
                GameFiber.Sleep(100);
            }
            sc.CallMethod("SET_VEHICLE_INFOR_AND_STATS", "Vacca", "Tracked and Registered", "MPCarHUD", "Pegassi", "Top Speed", "Accelrations", "Braking", "Traction", 21, 41, 61, 81);

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    Game.DisplayHelp(sc.MemoryAddress.ToString("X"));
                    sc.Draw(rect);
                    sc.Draw();
                    sc.Draw3D(Game.LocalPlayer.Character.GetOffsetPositionUp(5.45f), Rotator.Zero, new Vector3(12.0f, 9.0f, 1.0f));
                }
            });
        }
    }
}

