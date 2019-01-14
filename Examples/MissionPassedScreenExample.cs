namespace Examples
{
#if RPH1
    extern alias rph1;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
    using Game = rph1::Rage.Game;
    using MathHelper = rph1::Rage.MathHelper;
#else
    /** REDACTED **/
#endif

    using System;

    using RAGENativeUI;
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
            missionPassedScreen.Continued += (s, e) => Game.DisplayHelp("OnContinued");

            int i = 0;
            RPH.GameFiber.StartNew(() =>
            {
                while (true)
                {
                    RPH.GameFiber.Yield();

                    Game.DisplayHelp($"IsVisible: {missionPassedScreen.IsVisible}");

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        missionPassedScreen.Show();
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.H))
                    {
                        missionPassedScreen.Hide();
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.U))
                    {
                        missionPassedScreen.IsCompletionVisible = !missionPassedScreen.IsCompletionVisible;
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.J))
                    {
                        missionPassedScreen.ShownEffect = PostFxAnimation.GetByName(MathHelper.Choose("SuccessFranklin", "SuccessTrevor", "SuccessMichael", "SuccessNeutral"));
                        Game.DisplayNotification(missionPassedScreen.ShownEffect.Name);
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Add))
                    {
                        missionPassedScreen.Items.Add(new MissionPassedScreenItem("Item #" + i++, MathHelper.Choose("Status", "", ""), MathHelper.Choose((MissionPassedScreenItem.TickboxState[])Enum.GetValues(typeof(MissionPassedScreenItem.TickboxState)))));
                    }
                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Subtract))
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

