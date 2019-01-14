namespace Examples
{
#if RPH1
    extern alias rph1;
    using Game = rph1::Rage.Game;
    using Rotator = rph1::Rage.Rotator;
    using Vector3 = rph1::Rage.Vector3;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif
    
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
                RPH.GameFiber.Sleep(100);
            }
            sc.CallMethod("SET_VEHICLE_INFOR_AND_STATS", "Vacca", "Tracked and Registered", "MPCarHUD", "Pegassi", "Top Speed", "Accelrations", "Braking", "Traction", 21, 41, 61, 81);

            RPH.GameFiber.StartNew(() =>
            {
                while (true)
                {
                    RPH.GameFiber.Yield();
                    
                    sc.Draw(rect);
                    sc.Draw();
                    sc.Draw3D(Game.LocalPlayer.Character.GetOffsetPositionUp(5.45f), Rotator.Zero, new Vector3(12.0f, 9.0f, 1.0f));
                }
            });
        }
    }
}

