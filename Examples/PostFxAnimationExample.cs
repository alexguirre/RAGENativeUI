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
                    string s = "";
                    if (current != null)
                    {
                        s += $" Blend~n~";
                        s += $"     FrequencyNoise: {current.Blend.FrequencyNoise}~n~";
                        s += $"     AmplitudeNoise: {current.Blend.AmplitudeNoise}~n~";
                        s += $"     Frequency: {current.Blend.Frequency}~n~";
                        s += $"     Bias: {current.Blend.Bias}~n~";
                        s += $"     Disabled: {current.Blend.Disabled}~n~";
                        if (current.Blend.LayerA == null)
                        {
                            s += $"     LayerA: null~n~";
                        }
                        else
                        {
                            s += $"     LayerA:~n~";
                            s += $"         Mem: {current.Blend.LayerA.MemoryAddress.ToString("X8")}~n~";
                            s += $"         Mod: {(current.Blend.LayerA.Modifier == null ? "null" : current.Blend.LayerA.Modifier.Name)}~n~";
                            s += $"         AnimMode: {current.Blend.LayerA.AnimationMode}~n~";
                            s += $"         LoopMode: {current.Blend.LayerA.LoopMode}~n~";
                        }
                        s += "~n~";
                        if (current.Blend.LayerB == null)
                        {
                            s += $"     LayerB: null~n~";
                        }
                        else
                        {
                            s += $"     LayerB:~n~";
                            s += $"         Mem: {current.Blend.LayerB.MemoryAddress.ToString("X8")}~n~";
                            s += $"         Mod: {(current.Blend.LayerB.Modifier == null ? "null" : current.Blend.LayerB.Modifier.Name)}~n~";
                            s += $"         AnimMode: {current.Blend.LayerB.AnimationMode}~n~";
                            s += $"         LoopMode: {current.Blend.LayerB.LoopMode}~n~";
                        }
                        s += "~n~";
                    }
                    Game.DisplayHelp($"Name: {effect.Name}~n~Index: {effect.Index}~n~Active: {effect.IsActive.ToString()}~n~Valid: {effect.IsValid()}~n~MemAddress: {effect.MemoryAddress.ToString("X")}~n~Current: {(current != null ? current.MemoryAddress.ToString("X") : "null")}~n~{s}Last: {(last != null ? last.MemoryAddress.ToString("X") : "null")}");
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

