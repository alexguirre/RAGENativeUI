namespace Examples
{
    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;

    internal static class PostFxModifierExample
    {
        [ConsoleCommand(Name = "PostFxModifierExample", Description = "Example showing the PostFxModifier class.")]
        private static void Command()
        {
            PostFxModifier[] modifiers = PostFxModifier.GetAll();

            int i = 0;
            PostFxModifier mod = modifiers[i];

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.H))
                    {
                        foreach (PostFxModifier e in modifiers)
                        {
                            Game.Console.Print($"{e.Index}. {e.Name}");
                        }
                    }

                    PostFxModifier current = PostFxModifier.CurrentModifier;
                    Game.DisplayHelp($"Name: {mod.Name}~n~Index: {mod.Index}~n~Active: {mod.IsActive.ToString()}~n~Valid: {mod.IsValid()}~n~MemAddress: {mod.MemoryAddress.ToString("X")}~n~Current: {(current == null ? "null" : current.Name)}~n~CurrentIndex: {(current == null ? "-1" : current.Index.ToString())}");
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        mod.IsActive = !mod.IsActive;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        i = MathHelper.Clamp(i + 1, 0, modifiers.Length - 1);
                        mod.IsActive = false;
                        mod = modifiers[i];
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        i = MathHelper.Clamp(i - 1, 0, modifiers.Length - 1);
                        mod.IsActive = false;
                        mod = modifiers[i];
                    }
                }
            });
        }
    }
}

