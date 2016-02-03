[assembly: Rage.Attributes.Plugin("BigMessageExample", Author = "alexguirre", Description = "Example using RAGENativeUI")]

namespace BigMessageExampleProject
{
    using Rage;
    using RAGENativeUI.Elements;
    using System.Windows.Forms;

    public static class EntryPoint
    {
        private static BigMessageThread bigMessage;

        public static void Main()
        {
            bigMessage = new BigMessageThread(true);

            while (true)
            {
                if (Game.IsKeyDown(Keys.F5))
                    bigMessage.MessageInstance.ShowColoredShard("COLORED!", "I'm so colorful!", HudColor.HUD_COLOUR_BLUE, HudColor.HUD_COLOUR_GREEN);
                else if (Game.IsKeyDown(Keys.F6))
                    bigMessage.MessageInstance.ShowMissionPassedMessage("I passed the mission!");
                else if (Game.IsKeyDown(Keys.F7))
                    bigMessage.MessageInstance.ShowMpMessageLarge("This MultiPlayer Message is so large.");
                else if (Game.IsKeyDown(Keys.F8))
                    bigMessage.MessageInstance.ShowOldMessage("I'm old.");
                else if (Game.IsKeyDown(Keys.F9))
                    bigMessage.MessageInstance.ShowRankupMessage("RANK UP!", "IT'S OVER", 9000);
                else if (Game.IsKeyDown(Keys.F10))
                    bigMessage.MessageInstance.ShowSimpleShard("SIMPLE", "I'm so simple");
                else if (Game.IsKeyDown(Keys.F11))
                    bigMessage.MessageInstance.ShowWeaponPurchasedMessage("PURCHASE!", "Advanced Rifle", WeaponHash.AdvancedRifle);

                GameFiber.Yield();
            }
        }
    }
}
