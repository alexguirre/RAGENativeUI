namespace Examples
{
    using System;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class TimeCycleModifierExample
    {
        [ConsoleCommand(Name = "TimeCycleModifierExample", Description = "Example showing the TimeCycleModifier class.")]
        private static void Command()
        {
            int i = 0;
            TimeCycleModifier mod = TimeCycleModifier.GetByIndex(i);

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.H))
                    {
                        foreach (TimeCycleModifier e in TimeCycleModifier.GetAll())
                        {
                            Game.Console.Print($"{e.Index}. {e.Name}");
                        }
                    }

                    float y = 0.005f;
                    foreach (TimeCycleModifierMod m in mod.Mods)
                    {
                        string s = $"{m.Type}: {m.Value1.ToString("0.000")} {m.Value2.ToString("0.000")}~n~";
                        Text.Draw(ScreenPosition.FromRelativeCoords(0.98f, y), s, 0.3f, System.Drawing.Color.White, TextFont.ChaletLondon, TextAlignment.Right, 0.0f, false, true);
                        y += 0.0165f;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.U))
                    {
                        foreach (TimeCycleModifierMod m in mod.Mods)
                        {
                            m.Value1 = MathHelper.GetRandomSingle(0.0f, 15.0f);
                            m.Value2 = MathHelper.GetRandomSingle(0.0f, 15.0f);
                        }
                    }

                    TimeCycleModifier current = TimeCycleModifier.CurrentModifier;
                    Game.DisplayHelp($"Name: {mod.Name}~n~Index: {mod.Index}~n~Flags: {mod.Flags}~n~Active: {mod.IsActive.ToString()}~n~Valid: {mod.IsValid()}~n~MemAddress: {mod.MemoryAddress.ToString("X")}~n~Current: {(current == null ? "null" : current.Name)}~n~CurrentIndex: {(current == null ? "-1" : current.Index.ToString())}~n~Strength: {TimeCycleModifier.CurrentModifierStrength}~n~Count: {TimeCycleModifier.Count}");
                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Y))
                    {
                        mod.IsActive = !mod.IsActive;
                    }

                    if (Game.IsKeyDown(System.Windows.Forms.Keys.Add))
                    {
                        i++;
                        if (i >= TimeCycleModifier.Count)
                            i = 0;
                        TimeCycleModifier.CurrentModifier = null;
                        mod = TimeCycleModifier.GetByIndex(i);
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.Subtract))
                    {
                        i--;
                        if (i < 0)
                            i = TimeCycleModifier.Count - 1;
                        TimeCycleModifier.CurrentModifier = null;
                        mod = TimeCycleModifier.GetByIndex(i);
                    }


                    if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Multiply))
                    {
                        TimeCycleModifier.CurrentModifierStrength += 1.0f * Game.FrameTime;
                    }
                    else if (Game.IsKeyDownRightNow(System.Windows.Forms.Keys.Divide))
                    {
                        TimeCycleModifier.CurrentModifierStrength -= 1.0f * Game.FrameTime;
                    }


                    if (Game.IsKeyDown(System.Windows.Forms.Keys.J))
                    {
                        Game.DisplayNotification("Creating new CUSTOMCLONE" + TimeCycleModifier.Count);
                        Game.LogTrivial("Creating new CUSTOMCLONE" + TimeCycleModifier.Count);
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        TimeCycleModifier m = new TimeCycleModifier("CUSTOMCLONE" + TimeCycleModifier.Count, TimeCycleModifier.GetByIndex(4));
                        sw.Stop();
                        Game.LogTrivial($"It took {sw.ElapsedMilliseconds}ms/{sw.ElapsedTicks}ticks/{sw.Elapsed} to create TimeCycleModifier");
                    }
                    else if (Game.IsKeyDown(System.Windows.Forms.Keys.I))
                    {
                        Game.DisplayNotification("Creating new CUSTOM" + TimeCycleModifier.Count);
                        Game.LogTrivial("Creating new CUSTOM" + TimeCycleModifier.Count);
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        TimeCycleModifier m = new TimeCycleModifier("CUSTOM" + TimeCycleModifier.Count, 0, 
                           Tuple.Create(TimeCycleModifierModType.postfx_desaturation, MathHelper.GetRandomSingle(0.0f, 50.0f), 0.0f),
                           Tuple.Create(TimeCycleModifierModType.postfx_bright_pass_thresh, MathHelper.GetRandomSingle(0.0f, 50.0f), 0.0f));
                        sw.Stop();
                        Game.LogTrivial($"It took {sw.ElapsedMilliseconds}ms/{sw.ElapsedTicks}ticks/{sw.Elapsed} to create TimeCycleModifier");
                    }
                }
            });
        }
    }
}

