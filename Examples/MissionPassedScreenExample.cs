namespace Examples
{
    using System;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI.Scaleforms;

    internal static class MissionPassedScreenExample
    {
        [ConsoleCommand(Name = "MissionPassedScreenExample", Description = "Example showing the MissionPassedScreen class.")]
        private static void Command()
        {
            MissionPassedScreen missionPassedScreen = new MissionPassedScreen("mission passed", "Hotel Assassination");
            missionPassedScreen.Items.Add(new MissionPassedScreenItem("Time Taken", "04:18"));
            missionPassedScreen.Items.Add(new MissionPassedScreenItem("Sniper Kill Bonus", "", MissionPassedScreenItem.TickboxState.Tick));
            missionPassedScreen.Items.Add(new MissionPassedScreenItem("Money Earned", "$9,000", MissionPassedScreenItem.TickboxState.Tick));
            missionPassedScreen.IsCompletionVisible = true;
            missionPassedScreen.Continued += (s) => Game.DisplayHelp("OnContinued");

            int i = 0;
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    Game.DisplayHelp($"IsVisible: {missionPassedScreen.IsVisible}");

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        missionPassedScreen.Show();
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.H))
                    {
                        missionPassedScreen.Hide();
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.U))
                    {
                        missionPassedScreen.IsCompletionVisible = !missionPassedScreen.IsCompletionVisible;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.J))
                    {
                        missionPassedScreen.ScreenEffect = MathHelper.Choose("SuccessFranklin", "SuccessTrevor", "SuccessMichael", null);
                        Game.DisplayNotification(missionPassedScreen.ScreenEffect ?? "null");
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        missionPassedScreen.Items.Add(new MissionPassedScreenItem("Item #" + i++, MathHelper.Choose("Status", "", ""), MathHelper.Choose((MissionPassedScreenItem.TickboxState[])Enum.GetValues(typeof(MissionPassedScreenItem.TickboxState)))));
                    }
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        if (missionPassedScreen.Items.Count > 0)
                        {
                            missionPassedScreen.Items.RemoveAt(missionPassedScreen.Items.Count - 1);
                        }
                    }
                }
            });
        }
    }
}

