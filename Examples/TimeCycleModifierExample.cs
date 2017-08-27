namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using RAGENativeUI.ImGui;
    using RAGENativeUI.Rendering;

    internal static class TimeCycleModifierExample
    {
        [ConsoleCommand(Name = "TimeCycleModifierExample", Description = "Example showing the TimeCycleModifier class.")]
        private static void Command()
        {
            int i = 0;
            TimeCycleModifier mod = TimeCycleModifier.GetByIndex(i);

            RectangleF mainWindowRect = new RectangleF(10, 10, 350, 540);
            RectangleF modsValuesWindowRect = new RectangleF(Game.Resolution.Width - 455, 10, 445, 1000);
            bool showCurrentModsValues = false;
            
            Gui.Do += () =>
            {
                if (Game.IsControlKeyDownRightNow)
                {
                    Gui.Mouse();
                }

                mainWindowRect = Gui.Window(mainWindowRect, "Time Cycle Modifiers Example");

                Gui.Label(new RectangleF(3, mainWindowRect.Height - 50, 340, 20), "Hold down the CTRL key to enable the mouse", 11.5f, TextHorizontalAligment.Left, TextVerticalAligment.Top);

                if(Gui.Button(new RectangleF(5, 3, 122, 25), "Previous"))
                {
                    i--;
                    if (i < 0)
                        i = TimeCycleModifier.Count - 1;
                    TimeCycleModifier.CurrentModifier = null;
                    mod = TimeCycleModifier.GetByIndex(i);
                }

                if (Gui.Button(new RectangleF(5 + 3 + 122, 3, 122, 25), "Next"))
                {
                    i++;
                    if (i >= TimeCycleModifier.Count)
                        i = 0;
                    TimeCycleModifier.CurrentModifier = null;
                    mod = TimeCycleModifier.GetByIndex(i);
                }

                TimeCycleModifier current = TimeCycleModifier.CurrentModifier;
                Gui.Label(new RectangleF(5, 30, 350, 20), $"Current");
                Gui.Label(new RectangleF(15, 30 + 25 * 1, 350, 20), $"Name: {mod.Name}");
                Gui.Label(new RectangleF(15, 30 + 25 * 2, 350, 20), $"Index: {mod.Index}");
                Gui.Label(new RectangleF(15, 30 + 25 * 3, 350, 20), $"Flags: {mod.Flags}");
                Gui.Label(new RectangleF(15, 30 + 25 * 4, 350, 20), $"Active: {mod.IsActive}");
                Gui.Label(new RectangleF(15, 30 + 25 * 5, 350, 20), $"Memory Address: {mod.MemoryAddress.ToString("X")}");

                Gui.Label(new RectangleF(5, 30 + 25 * 7, 350, 20), $"Current Strength: {TimeCycleModifier.CurrentModifierStrength}");
                TimeCycleModifier.CurrentModifierStrength = Gui.HorizontalSlider(new RectangleF(5, 30 + 25 * 8, 340, 25), TimeCycleModifier.CurrentModifierStrength, -1.0f, 5.0f);

                Gui.Label(new RectangleF(5, 30 + 25 * 10, 350, 20), $"Total Count: {TimeCycleModifier.Count}");

                mod.IsActive = Gui.Toggle(new RectangleF(5, 30 * 11, 150, 25), $"Active: {mod.IsActive}", mod.IsActive);

                if (Gui.Button(new RectangleF(5, 30 * 12, 250, 25),  $"{(showCurrentModsValues ? "Hide" : "Show")} Current Mods Values"))
                {
                    showCurrentModsValues = !showCurrentModsValues;
                }

                if (Gui.Button(new RectangleF(5, 30 * 13, 250, 25), "Randomize Current Mods Values"))
                {
                    foreach (TimeCycleModifierMod m in mod.Mods)
                    {
                        m.Value1 = MathHelper.GetRandomSingle(0.0f, 15.0f);
                        m.Value2 = MathHelper.GetRandomSingle(0.0f, 15.0f);
                    }
                }

                if (Gui.Button(new RectangleF(5, 30 * 14, 250, 25), "Clone Current"))
                {
                    Game.DisplayNotification("Creating new CLONE" + TimeCycleModifier.Count);
                    Game.LogTrivial("Creating new CLONE" + TimeCycleModifier.Count);
                    TimeCycleModifier m = new TimeCycleModifier("CLONE" + TimeCycleModifier.Count, mod);
                }

                if (Gui.Button(new RectangleF(5, 30 * 15, 250, 25), "New Modifier"))
                {
                    Game.DisplayNotification("Creating new CUSTOM" + TimeCycleModifier.Count);
                    Game.LogTrivial("Creating new CUSTOM" + TimeCycleModifier.Count);
                    TimeCycleModifier m = new TimeCycleModifier("CUSTOM" + TimeCycleModifier.Count, 0,
                       Tuple.Create(TimeCycleModifierModType.postfx_desaturation, MathHelper.GetRandomSingle(0.0f, 50.0f), 0.0f),
                       Tuple.Create(TimeCycleModifierModType.postfx_bright_pass_thresh, MathHelper.GetRandomSingle(0.0f, 50.0f), 0.0f));
                }
            };

            Gui.Do += () =>
            {
                if (showCurrentModsValues)
                {
                    if (Game.IsControlKeyDownRightNow)
                    {
                        Gui.Mouse();
                    }

                    modsValuesWindowRect = Gui.Window(modsValuesWindowRect, "Mods Values");

                    Gui.Label(new RectangleF(3, 3, 434, 25), "Mods Count:");
                    Gui.Label(new RectangleF(3, 3, 434, 25), $"{mod.Mods.Count}", 15.0f, TextHorizontalAligment.Right);

                    float y = 30;
                    foreach (TimeCycleModifierMod m in mod.Mods)
                    {
                        Gui.Label(new RectangleF(3, y, 434, 25), $"{m.Type}:");
                        Gui.Label(new RectangleF(3, y, 434, 25), $"{m.Value1.ToString("0.000")} {m.Value2.ToString("0.000")}", 15.0f, TextHorizontalAligment.Right);
                        y += 26;
                    }
                }
            };
        }
    }
}

