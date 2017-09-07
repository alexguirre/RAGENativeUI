namespace RAGENativeUI.Elements
{
    using System.Drawing;
    
    using Rage.Native;

    public class Box
    {
        public bool IsVisible { get; set; } = true;
        public ScreenRectangle Rectangle { get; set; }
        public Color Color { get; set; }

        public Box(ScreenRectangle rectangle, Color color)
        {
            Rectangle = rectangle;
            Color = color;
        }

        public void Draw()
        {
            if (!IsVisible)
                return;

            Draw(Rectangle, Color);
        }


        public static void Draw(ScreenRectangle rectangle, Color color)
        {
            NativeFunction.Natives.DrawRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color.R, color.G, color.B, color.A);
        }
    }
}

