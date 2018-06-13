namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Attributes;

    using RAGENativeUI;
    using RAGENativeUI.ImGui;

    internal static class PostFxAnimationExample
    {
        [ConsoleCommand(Name = "PostFxAnimationExample", Description = "Example showing the PostFxAnimation class.")]
        private static void Command()
        {
            PostFxAnimation[] effects = PostFxAnimation.GetAll();

            int i = PostFxAnimation.LastAnimation == null ? 0 : PostFxAnimation.LastAnimation.Index;
            PostFxAnimation effect = effects[i];

            RectangleF mainWindowRect = new RectangleF(10, 10, 360, 270);
            RectangleF effectInfoWindowRect = new RectangleF(Game.Resolution.Width - 400, 10, 360, 650);

            int duration = 1000;
            bool looped = false;

            bool showEffectInfo = false;

            Gui.Do += () =>
            {
                if (Game.IsControlDown)
                {
                    Gui.Mouse();
                }

                mainWindowRect = Gui.BeginWindow(mainWindowRect, "Post Fx Animations Example");

                Gui.Label(new RectangleF(3, mainWindowRect.Height - 50, 340, 20), "Hold down the CTRL key to enable the mouse", 11.5f, TextHorizontalAligment.Left, TextVerticalAligment.Top);

                if (Gui.Button(new RectangleF(5, 3, 122, 25), "Previous"))
                {
                    i--;
                    if (i < 0)
                        i = effects.Length - 1;
                    effect.Stop();
                    effect = effects[i];
                }

                if (Gui.Button(new RectangleF(5 + 3 + 122, 3, 122, 25), "Next"))
                {
                    i++;
                    if (i >= effects.Length)
                        i = 0;
                    effect.Stop();
                    effect = effects[i];
                }

                Gui.Label(new RectangleF(5, 30, 350, 20), $"Name: {effect.Name}");
                Gui.Label(new RectangleF(5, 55, 350, 20), $"Index: {effect.Index}");

                Gui.Label(new RectangleF(5, 90, 350, 20), $"Duration: {(duration == 0 ? "Default" : duration.ToString())}");
                duration = Gui.HorizontalSlider(new RectangleF(5, 115, 350, 20), duration, 0, 15000);

                looped = Gui.Toggle(new RectangleF(5, 140, 60, 20), $"Loop", looped);

                bool active = effect.IsActive;
                if (Gui.Button(new RectangleF(5, 165, 350, 20), active ? "Stop" : "Start"))
                {
                    if (active)
                        effect.Stop();
                    else
                        effect.Start(duration, looped);
                }
                

                if (Gui.Button(new RectangleF(5, 190, 350, 20), showEffectInfo ? "Hide Info" : "Show Info"))
                {
                    showEffectInfo = !showEffectInfo;
                }

                Gui.EndWindow();

                if (showEffectInfo)
                {
                    effectInfoWindowRect = Gui.BeginWindow(effectInfoWindowRect, "Current Post Fx Animation Info");
                    
                    string s = "";
                    s += $"Name: {effect.Name}\r\n";
                    s += $"Hash: 0x{effect.Hash.ToString("X8")}\r\n";
                    s += $"Index: {effect.Index}\r\n";
                    s += $"Active: {effect.IsActive}\r\n";
                    s += $"Valid: {effect.IsValid()}\r\n";
                    s += $"Memory Address: {effect.MemoryAddress.ToString("X")}\r\n";
                    s += $"Layers Count: {effect.Layers.Count}\r\n";
                    s += $"Blend:\r\n";
                    s += $" FrequencyNoise: {effect.LayerBlend.FrequencyNoise}\r\n";
                    s += $" AmplitudeNoise: {effect.LayerBlend.AmplitudeNoise}\r\n";
                    s += $" Frequency: {effect.LayerBlend.Frequency}\r\n";
                    s += $" Bias: {effect.LayerBlend.Bias}\r\n";
                    s += $" Disabled: {effect.LayerBlend.Disabled}\r\n";
                    if (effect.LayerBlend.LayerA == null)
                    {
                        s += $" LayerA: null\r\n";
                    }
                    else
                    {
                        s += $" Layer A:\r\n";
                        s += $"   Memory Address: {effect.LayerBlend.LayerA.MemoryAddress.ToString("X")}\r\n";
                        s += $"   TimeCycle Modifier: {(effect.LayerBlend.LayerA.Modifier == null ? "null" : effect.LayerBlend.LayerA.Modifier.Name)}\r\n";
                        s += $"   Animation Mode: {effect.LayerBlend.LayerA.AnimationMode}\r\n";
                        s += $"   Loop Mode: {effect.LayerBlend.LayerA.LoopMode}\r\n";
                        s += $"   Start Delay Duration: {effect.LayerBlend.LayerA.StartDelayDuration}\r\n";
                        s += $"   In Duration: {effect.LayerBlend.LayerA.InDuration}\r\n";
                        s += $"   Hold Duration: {effect.LayerBlend.LayerA.HoldDuration}\r\n";
                        s += $"   Out Duration: {effect.LayerBlend.LayerA.OutDuration}\r\n";
                    }
                    s += "\r\n";
                    if (effect.LayerBlend.LayerB == null)
                    {
                        s += $" LayerB: null\r\n";
                    }
                    else
                    {
                        s += $" Layer B:\r\n";
                        s += $"   Memory Address: {effect.LayerBlend.LayerB.MemoryAddress.ToString("X")}\r\n";
                        s += $"   TimeCycle Modifier: {(effect.LayerBlend.LayerB.Modifier == null ? "null" : effect.LayerBlend.LayerB.Modifier.Name)}\r\n";
                        s += $"   Animation Mode: {effect.LayerBlend.LayerB.AnimationMode}\r\n";
                        s += $"   Loop Mode: {effect.LayerBlend.LayerB.LoopMode}\r\n";
                        s += $"   Start Delay Duration: {effect.LayerBlend.LayerB.StartDelayDuration}\r\n";
                        s += $"   In Duration: {effect.LayerBlend.LayerB.InDuration}\r\n";
                        s += $"   Hold Duration: {effect.LayerBlend.LayerB.HoldDuration}\r\n";
                        s += $"   Out Duration: {effect.LayerBlend.LayerB.OutDuration}\r\n";
                    }

                    Gui.Label(new RectangleF(5, 5, 350, 640), s, 15.0f, TextHorizontalAligment.Left, TextVerticalAligment.Top);

                    Gui.EndWindow();
                }
            };
        }
    }
}

