namespace Examples
{
    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Scaleforms;

    internal static class ScaleformExample
    {
        [ConsoleCommand(Name = "ScaleformExample", Description = "Example showing the Scaleform class.")]
        private static void Command()
        {
            ScreenRectangle rect = ScreenRectangle.FromRelativeCoords(0.5f, 0.5f, 0.5f, 0.5f);
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

                    Game.DisplayHelp(sc.MemoryAddress.ToString("X") + "~n~" + sc.BoundingBoxColor);
                    sc.Draw(rect);
                    sc.Draw();
                    sc.Draw3D(Game.LocalPlayer.Character.GetOffsetPositionUp(5.45f), Rotator.Zero, new Vector3(12.0f, 9.0f, 1.0f));

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        sc.BoundingBoxColor = System.Drawing.Color.FromArgb(MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256), MathHelper.GetRandomInteger(256));
                    }
                }
            });
        }
    }
}

