namespace Examples
{
    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;

    internal static class TimeCycleModifierExample
    {
        [ConsoleCommand(Name = "TimeCycleModifierExample", Description = "Example showing the TimeCycleModifier class.")]
        private static void Command()
        {
            TimeCycleModifier[] modifiers = TimeCycleModifier.GetAll();

            int i = 0;
            TimeCycleModifier mod = modifiers[i];

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.H))
                    {
                        foreach (TimeCycleModifier e in modifiers)
                        {
                            Game.Console.Print($"{e.Index}. {e.Name}");
                        }
                    }

                    TimeCycleModifier current = TimeCycleModifier.CurrentModifier;
                    Game.DisplayHelp($"Name: {mod.Name}~n~Index: {mod.Index}~n~Active: {mod.IsActive.ToString()}~n~Valid: {mod.IsValid()}~n~MemAddress: {mod.MemoryAddress.ToString("X")}~n~Current: {(current == null ? "null" : current.Name)}~n~CurrentIndex: {(current == null ? "-1" : current.Index.ToString())}~n~Strength: {TimeCycleModifier.CurrentModifierStrength}");
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        mod.IsActive = !mod.IsActive;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        i = MathHelper.Clamp(i + 1, 0, modifiers.Length - 1);
                        TimeCycleModifier.CurrentModifier = null;
                        mod = modifiers[i];
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        i = MathHelper.Clamp(i - 1, 0, modifiers.Length - 1);
                        TimeCycleModifier.CurrentModifier = null;
                        mod = modifiers[i];
                    }


                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Multiply))
                    {
                        TimeCycleModifier.CurrentModifierStrength += 1.0f * Game.FrameTime;
                    }
                    else if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Divide))
                    {
                        TimeCycleModifier.CurrentModifierStrength -= 1.0f * Game.FrameTime;
                    }
                }
            });
        }
    }
}

