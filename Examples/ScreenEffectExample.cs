namespace Examples
{
    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;

    internal static class ScreenEffectExample
    {
        [ConsoleCommand(Name = "ScreenEffectExample", Description = "Example showing the ScreenEffect class.")]
        private static void Command()
        {
            ScreenEffect[] effects = ScreenEffect.GetAll();
            int i = 0;
            ScreenEffect effect = effects[i];
            
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();


                    if (Game.IsKeyDown(System.Windows.Forms.Keys.H))
                    {
                        foreach (ScreenEffect e in effects)
                        {
                            Game.Console.Print(e.Name);
                        }
                    }

                    ScreenEffect current = ScreenEffect.CurrentActive;
                    ScreenEffect last = ScreenEffect.LastActive;
                    Game.DisplayHelp($"Name: {effect.Name}~n~Idx: {i}~n~Active: {effect.IsActive.ToString()}~n~Valid: {effect.IsValid()}~n~MemAddress: {effect.MemoryAddress.ToString("X")}~n~Current: {(current != null ? current.MemoryAddress.ToString("X") : "null")}~n~Last: {(last != null ? last.MemoryAddress.ToString("X") : "null")}");
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        if (effect.IsActive)
                            effect.Stop();
                        else
                            effect.Start(500, false);
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        i = MathHelper.Clamp(i + 1, 0, effects.Length - 1);
                        effect.Stop();
                        effect = effects[i];
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        i = MathHelper.Clamp(i - 1, 0, effects.Length - 1);
                        effect.Stop();
                        effect = effects[i];
                    }
                }
            });
        }
    }
}

