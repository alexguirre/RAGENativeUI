namespace RNUIExamples
{
    using Rage;
    using Rage.Attributes;
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class BigMessageExample
    {
        private static void Main()
        {
            // create the big message handler
            BigMessageThread bigMessageThread = new BigMessageThread(true);
            BigMessageHandler bigMessage = bigMessageThread.MessageInstance;

            // continue with the plugin...
            Game.Console.Print("  When your wanted level raises, a big message is shown.");

            int lastWantedLevel = 0;
            while (true)
            {
                GameFiber.Yield();


                int wantedLevel = Game.LocalPlayer.WantedLevel;

                if (lastWantedLevel != wantedLevel)
                {
                    if (wantedLevel != 0)
                    {
                        // show a message when the wanted level changes
                        bigMessage.ShowColoredShard("WANTED", $"{wantedLevel} stars", HudColor.Gold, HudColor.InGameBackground);
                    }

                    lastWantedLevel = wantedLevel;
                }
            }
        }


        // a command that simulates loading the plugin
        [ConsoleCommand]
        private static void RunBigMessageExample() => GameFiber.StartNew(Main);
    }
}
