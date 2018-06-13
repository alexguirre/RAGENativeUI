namespace Examples
{
    using System;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Scaleforms;

    internal static class BigMessageExample
    {
        [ConsoleCommand(Name = "BigMessageExample", Description = "Example showing the BigMessage class.")]
        private static void Command()
        {
            BigMessage bigMessage = new BigMessage();

            GameFiber.StartNew(() =>
            {
                float y = 0f;
                while (true)
                {
                    GameFiber.Yield();

                    bigMessage.Draw();

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.D1))
                    {
                        bigMessage.ShowMissionPassedMessage("message");
                    }
                    else if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.D2))
                    {
                        bigMessage.ShowMissionPassedOldMessage("message", "subtitle");
                    }
                    else if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.D3))
                    {
                        bigMessage.ShowColoredShard("message", "subtitle", HudColor.RedDark, HudColor.White);
                    }
                    else if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.D4))
                    {
                        bigMessage.ShowMpMessageLarge("message", "subtitle");
                    }
                    else if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.D5))
                    {
                        bigMessage.ShowRankupMessage("message", "subtitle", 42);
                    }
                    else if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.D6))
                    {
                        bigMessage.ShowSimpleShard("message", "subtitle");
                    }
                    else if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.D7))
                    {
                        bigMessage.ShowWeaponPurchasedMessage("message", "weapon name", WeaponHash.AssaultSMG);
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        bigMessage.OutTransition = MathHelper.Choose((BigMessage.OutTransitionType[])Enum.GetValues(typeof(BigMessage.OutTransitionType)));
                        Game.DisplayNotification(bigMessage.OutTransition.ToString());
                    }

                    if (Game.WasKeyJustPressed(System.Windows.Forms.Keys.H))
                    {
                        bigMessage.Hide();
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        bigMessage.SetVerticlePositionOverride(y += 0.5f * Game.FrameTime);
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        bigMessage.SetVerticlePositionOverride(y -= 0.5f * Game.FrameTime);
                    }
                }
            });
        }
    }
}

