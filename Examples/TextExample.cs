namespace Examples
{
#if RPH1
    extern alias rph1;
    using ConsoleCommandAttribute = rph1::Rage.Attributes.ConsoleCommandAttribute;
#else
    /** REDACTED **/
#endif

    using System.Drawing;
    
    using RAGENativeUI;
    using RAGENativeUI.Elements;

    internal static class TextExample
    {
        [ConsoleCommand(Name = "TextExample", Description = "Example showing the Text class.")]
        private static void Command()
        {
            Text text1 = new Text("Left", ScreenPosition.FromRelativeCoords(0.5f, 0.25f), 1.0f) { IsVisible = true, Alignment = TextAlignment.Left };
            Text text2 = new Text("Center", ScreenPosition.FromRelativeCoords(0.5f, 0.5f), 1.0f) { IsVisible = true, Alignment = TextAlignment.Center };
            Text text3 = new Text("Right", ScreenPosition.FromRelativeCoords(0.5f, 0.75f), 1.0f) { IsVisible = true, Alignment = TextAlignment.Right };
            Text text4 = new Text("[LEFT] Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.",
                                  ScreenPosition.FromRelativeCoords(0.5f, 0.025f), 0.6f) { IsVisible = true, Alignment = TextAlignment.Left, Color = Color.Red, DropShadow = true, Outline = true };
            Text text5 = new Text("[RIGHT] Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.",
                                  ScreenPosition.FromRelativeCoords(0.3f, 0.25f), 0.6f) { IsVisible = true, Alignment = TextAlignment.Right, Color = Color.Orange, Outline = true };
            Text text6 = new Text("[CENTER] Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.",
                                  ScreenPosition.FromRelativeCoords(0.75f, 0.25f), 0.6f) { IsVisible = true, Alignment = TextAlignment.Center, Color = Color.Green, Outline = true, WrapWidth = 0.25f };
            Text text7 = new Text(new string(System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Repeat('a', 396))),
                                  ScreenPosition.FromRelativeCoords(0.5f, 0.05f), 0.2f) { IsVisible = true, Alignment = TextAlignment.Left, Color = Color.Green, Outline = true};

            RPH.GameFiber.StartNew(() =>
            {
                while (true)
                {
                    RPH.GameFiber.Yield();
                    
                    text1.Draw();
                    text2.Draw();
                    text3.Draw();
                    text4.Draw();
                    text5.Draw();
                    text6.Draw();
                    text7.Draw();
                }
            });
        }
    }
}

