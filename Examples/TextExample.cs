namespace Examples
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;
    using Rage.Attributes;
    using Graphics = Rage.Graphics;

    using RAGENativeUI;
    using RAGENativeUI.Elements;
    using RAGENativeUI.Utility;

    internal static class TextExample
    {
        [ConsoleCommand(Name = "TextExample", Description = "Example showing the Text class.")]
        private static void Command()
        {
            GameScreenRectangle fromAbsolute = GameScreenRectangle.FromAbsoluteCoords(1920f / 4f, 1080f / 4f, 1920f / 2f, 1080f / 2f);
            GameScreenRectangle fromRelative = GameScreenRectangle.FromRelativeCoords(0.5f, 0.5f, 0.5f, 0.5f);

            Text text1 = new Text("Left", GameScreenPosition.FromRelativeCoords(0.5f, 0.25f), 1.0f) { IsVisible = true, Alignment = TextAlignment.Left };
            Text text2 = new Text("Center", GameScreenPosition.FromRelativeCoords(0.5f, 0.5f), 1.0f) { IsVisible = true, Alignment = TextAlignment.Center };
            Text text3 = new Text("Right", GameScreenPosition.FromRelativeCoords(0.5f, 0.75f), 1.0f) { IsVisible = true, Alignment = TextAlignment.Right };
            Text text4 = new Text("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.",
                                  GameScreenPosition.FromRelativeCoords(0.025f, 0.025f), 0.6f) { IsVisible = true, Alignment = TextAlignment.Left, Color = Color.Red, DropShadow = true, Outline = true };
            Text text5 = new Text("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.",
                                  GameScreenPosition.FromRelativeCoords(0.3f, 0.25f), 0.6f) { IsVisible = true, Alignment = TextAlignment.Right, Color = Color.Orange, Outline = true };
            Text text6 = new Text("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim.",
                                  GameScreenPosition.FromRelativeCoords(0.75f, 0.25f), 0.6f) { IsVisible = true, Alignment = TextAlignment.Center, Color = Color.Green, Outline = true, WrapWidth = 0.25f };

            GameFiber.StartNew(() =>
            {
                while (true)
                {
                    GameFiber.Yield();
                    
                    text1.Draw();
                    text2.Draw();
                    text3.Draw();
                    text4.Draw();
                    text5.Draw();
                    text6.Draw();
                }
            });
        }
    }
}

