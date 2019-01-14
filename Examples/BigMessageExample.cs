namespace Examples
{
#if RPH1
    extern alias rph1;
    using GameFiber = rph1::Rage.GameFiber;
    using Game = rph1::Rage.Game;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
    using WeaponHash = rph1::Rage.WeaponHash;
    using MathHelper = rph1::Rage.MathHelper;
#else
    /** REDACTED **/
#endif

    using System;
    
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
                    RPH.GameFiber.Yield();

                    bigMessage.Draw();

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.D1))
                    {
                        bigMessage.ShowMissionPassedMessage("message");
                    }
                    else if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.D2))
                    {
                        bigMessage.ShowMissionPassedOldMessage("message", "subtitle");
                    }
                    else if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.D3))
                    {
                        bigMessage.ShowColoredShard("message", "subtitle", HudColor.RedDark, HudColor.White);
                    }
                    else if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.D4))
                    {
                        bigMessage.ShowMpMessageLarge("message", "subtitle");
                    }
                    else if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.D5))
                    {
                        bigMessage.ShowRankupMessage("message", "subtitle", 42);
                    }
                    else if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.D6))
                    {
                        bigMessage.ShowSimpleShard("message", "subtitle");
                    }
                    else if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.D7))
                    {
                        bigMessage.ShowWeaponPurchasedMessage("message", "weapon name", WeaponHash.AssaultSMG);
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.Y))
                    {
                        bigMessage.OutTransition = MathHelper.Choose((BigMessage.OutTransitionType[])Enum.GetValues(typeof(BigMessage.OutTransitionType)));
                        Game.DisplayNotification(bigMessage.OutTransition.ToString());
                    }

                    if (RPH.Game.WasKeyJustPressed(System.Windows.Forms.Keys.H))
                    {
                        bigMessage.Hide();
                    }

                    if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        bigMessage.SetVerticlePositionOverride(y += 0.5f * Game.FrameTime);
                    }
                    else if (RPH.Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        bigMessage.SetVerticlePositionOverride(y -= 0.5f * Game.FrameTime);
                    }
                }
            });
        }
    }
}

