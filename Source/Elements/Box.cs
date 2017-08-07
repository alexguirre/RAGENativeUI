namespace RAGENativeUI.Elements
{
    using System.Drawing;
    
    using Rage.Native;

    public class Box
    {
        public bool IsVisible { get; set; }
        public GameScreenRectangle Rectangle { get; set; }
        public Color Color { get; set; }

        public Box(GameScreenRectangle rectangle, Color color)
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


        public static void Draw(GameScreenRectangle rectangle, Color color)
        {
            NativeFunction.Natives.DrawRect(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color.R, color.G, color.B, color.A);
        }
    }
}

