namespace Examples
{
#if RPH1
    extern alias rph1;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

    using System;
    using System.Drawing;
    
    using RAGENativeUI;
    using RAGENativeUI.Drawing;

    internal static class TextExample
    {
        [ConsoleCommand(Name = "TextExample", Description = "Example showing the Text class.")]
        private static void Command()
        {
            Text text1 = new Text((0.5f, 0.25f).Rel(), 1.0f) { IsVisible = true, Alignment = TextAlignment.Left };
            text1.SetText("Left");
            Text text2 = new Text((0.5f, 0.5f).Rel(), 1.0f) { IsVisible = true, Alignment = TextAlignment.Center };
            text2.SetText("Center");
            Text text3 = new Text((0.5f, 0.75f).Rel(), 1.0f) { IsVisible = true, Alignment = TextAlignment.Right };
            text3.SetText("Right");
            Text text4 = new Text((0.5f, 0.025f).Rel(), 0.6f) { IsVisible = true, Alignment = TextAlignment.Left, Color = Color.Red, DropShadow = true, Outline = true };
            text4.SetText("[LEFT] Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.");
            Text text5 = new Text((0.3f, 0.25f).Rel(), 0.6f) { IsVisible = true, Alignment = TextAlignment.Right, Color = Color.Orange, Outline = true };
            text5.SetText("[RIGHT] Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.");
            Text text6 = new Text((0.75f, 0.25f).Rel(), 0.6f) { IsVisible = true, Alignment = TextAlignment.Center, Color = Color.Green, Outline = true, WrapWidth = 0.25f };
            text6.SetText("[CENTER] Lorem ipsum dolor sit amet, consectetuer adipiscing elit.Aenean commodo ligula eget dolor.Aenean massa.Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem.Nulla consequat massa quis enim.");
            Text text7 = new Text("HUD_CASH", (0.1f, 0.1f).Rel(), 0.6f) { IsVisible = true, Alignment = TextAlignment.Left, Color = Color.Green, Outline = true };
            text7.AddComponentFloat(11500.4233f, 2);
            Text text8 = new Text("STRING", (0.1f, 0.15f).Rel(), 0.6f) { IsVisible = true, Alignment = TextAlignment.Left, Color = Color.Green, Outline = true };
            text8.AddComponentTime(TimeSpan.Zero, TextComponentTimeOptions.Hours | TextComponentTimeOptions.Milliseconds | TextComponentTimeOptions.UseDotAsMillisecondsSeparator);

            RPH.GameFiber.StartNew(() =>
            {
                var v = 0.0f;
                while (true)
                {
                    RPH.GameFiber.Yield();

                    (text8.Components[0] as TextComponentTime).Time = DateTime.Now.TimeOfDay;

                    text1.Draw();
                    text2.Draw();
                    text3.Draw();
                    text4.Draw();
                    text5.Draw();
                    text6.Draw();
                    text7.Draw();
                    text8.Draw();
                }
            });
        }
    }
}

