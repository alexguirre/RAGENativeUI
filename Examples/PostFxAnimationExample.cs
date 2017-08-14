namespace Examples
{
    using System;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;

    internal static class PostFxAnimationExample
    {
        [ConsoleCommand(Name = "PostFxAnimationExample", Description = "Example showing the PostFxAnimation class.")]
        private static void Command()
        {
            PostFxAnimation[] effects = PostFxAnimation.GetAll();

            int i = PostFxAnimation.LastAnimation == null ? 0 : PostFxAnimation.LastAnimation.Index;
            PostFxAnimation effect = effects[i];
            
            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.H))
                    {
                        foreach (PostFxAnimation e in effects)
                        {
                            Game.Console.Print($"{e.Index}. {e.Name}");
                        }
                    }

                    PostFxAnimation current = PostFxAnimation.CurrentAnimation;
                    PostFxAnimation last = PostFxAnimation.LastAnimation;
                    Game.DisplayHelp($"Name: {effect.Name}~n~Index: {effect.Index}~n~Active: {effect.IsActive.ToString()}~n~Valid: {effect.IsValid()}~n~MemAddress: {effect.MemoryAddress.ToString("X")}~n~Current: {(current != null ? current.MemoryAddress.ToString("X") : "null")}~n~Last: {(last != null ? last.MemoryAddress.ToString("X") : "null")}");
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

