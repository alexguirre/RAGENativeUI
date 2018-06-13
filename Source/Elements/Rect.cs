namespace RAGENativeUI.Elements
{
    using System.Drawing;
    
    public class Rect
    {
        public bool IsVisible { get; set; } = true;
        public ScreenRectangle Rectangle { get; set; }
        public Color Color { get; set; }

        public Rect(ScreenRectangle rectangle, Color color)
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
            N.DrawRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color.R, color.G, color.B, color.A);
        }
    }
}

