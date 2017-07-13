namespace RAGENativeUI.Elements
{
    using System;
    using System.Drawing;

    using Rage;
    using Rage.Native;

    using RAGENativeUI.Utility;

    public class Rectangle
    {
        public bool IsVisible { get; set; }
        public GameScreenRectangle ScreenRectangle { get; set; }
        public Color Color { get; set; }

        public Rectangle(GameScreenRectangle rectangle, Color color)
        {
            ScreenRectangle = rectangle;
            Color = color;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(ScreenRectangle, Color);
        }


        public static void Draw(GameScreenRectangle rectangle, Color color)
        {
            NativeFunction.Natives.DrawRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color.R, color.G, color.B, color.A);
        }
    }
}

